using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers.OrderStartedEvent
{
    public class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler 
                        : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IIdentityService _identityService;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(
            ILoggerFactory logger, 
            IBuyerRepository buyerRepository, 
            IIdentityService identityService,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStartedDomainEvent orderStartedEvent, CancellationToken cancellationToken)
        {            
            var cardTypeId = (orderStartedEvent.CardTypeId != 0) ? orderStartedEvent.CardTypeId : 1;
            var buyer = await _buyerRepository.FindAsync(orderStartedEvent.UserId);
            bool buyerOriginallyExisted = (buyer == null) ? false : true;

            if (!buyerOriginallyExisted)
            {                
                buyer = new Buyer(orderStartedEvent.UserId, orderStartedEvent.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                           $"Payment Method on {DateTime.UtcNow}",
                                           orderStartedEvent.CardNumber,
                                           orderStartedEvent.CardSecurityNumber,
                                           orderStartedEvent.CardHolderName,
                                           orderStartedEvent.CardExpiration,
                                           orderStartedEvent.Order.Id);

            var buyerUpdated = buyerOriginallyExisted ? 
                _buyerRepository.Update(buyer) : 
                _buyerRepository.Add(buyer);

            await _buyerRepository.UnitOfWork
                .SaveEntitiesAsync();

            var orderStatusChangedTosubmittedIntegrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(orderStartedEvent.Order.Id, orderStartedEvent.Order.OrderStatus.Name, buyer.Name);
            await _orderingIntegrationEventService.PublishThroughEventBusAsync(orderStatusChangedTosubmittedIntegrationEvent);

            _logger.CreateLogger(nameof(ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler)).LogTrace($"Buyer {buyerUpdated.Id} and related payment method were validated or updated for orderId: {orderStartedEvent.Order.Id}.");
        }
    }
}
