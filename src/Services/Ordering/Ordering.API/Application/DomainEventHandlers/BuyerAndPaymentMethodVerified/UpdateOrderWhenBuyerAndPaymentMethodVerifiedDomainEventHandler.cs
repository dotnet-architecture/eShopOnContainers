using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF.Services;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using Microsoft.Extensions.Logging;
using Ordering.API.IntegrationEvents.Events;
using Ordering.Domain.Events;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers.BuyerAndPaymentMethodVerified
{
    public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler 
                   : IAsyncNotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;

        public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger, IEventBus eventBus,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Domain Logic comment:
        // When the Buyer and Buyer's payment method have been created or verified that they existed, 
        // then we can update the original Order with the BuyerId and PaymentId (foreign keys)
        public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent buyerPaymentMethodVerifiedEvent)
        {
            var orderToUpdate = await _orderRepository.GetAsync(buyerPaymentMethodVerifiedEvent.OrderId);
            orderToUpdate.SetBuyerId(buyerPaymentMethodVerifiedEvent.Buyer.Id);
            orderToUpdate.SetPaymentId(buyerPaymentMethodVerifiedEvent.Payment.Id);
                                    
            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(buyerPaymentMethodVerifiedEvent.Buyer.IdentityGuid);
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            var orderingContext = _orderRepository.UnitOfWork as OrderingContext;
            var strategy = orderingContext.Database.CreateExecutionStrategy();

            var eventLogService = _integrationEventLogServiceFactory(orderingContext.Database.GetDbConnection());
            await strategy.ExecuteAsync(async () =>
            {
            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            using (var transaction = orderingContext.Database.BeginTransaction())
                {
                    await _orderRepository.UnitOfWork.SaveEntitiesAsync();
                    await eventLogService.SaveEventAsync(orderStartedIntegrationEvent, orderingContext.Database.CurrentTransaction.GetDbTransaction());
                    transaction.Commit();
                }
            });

            _logger.CreateLogger(nameof(UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler))
                .LogTrace($"Order with Id: {buyerPaymentMethodVerifiedEvent.OrderId} has been successfully updated with a payment method id: { buyerPaymentMethodVerifiedEvent.Payment.Id }");

            _eventBus.Publish(orderStartedIntegrationEvent);
            await eventLogService.MarkEventAsPublishedAsync(orderStartedIntegrationEvent);            
        }
    }  
}
