using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;

namespace Ordering.API.Application.DomainEventHandlers.BuyerAndPaymentMethodVerified
{
    public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler 
                   : INotificationHandler<DomainEventNotification<BuyerAndPaymentMethodVerifiedDomainEvent>>
    {
        private readonly IOrderRepository _orderRepository;        
        private readonly ILoggerFactory _logger;        

        public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger)            
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Domain Logic comment:
        // When the Buyer and Buyer's payment method have been created or verified that they existed, 
        // then we can update the original Order with the BuyerId and PaymentId (foreign keys)
        public async Task Handle(DomainEventNotification<BuyerAndPaymentMethodVerifiedDomainEvent> buyerPaymentMethodVerifiedEventNotification, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.GetAsync(buyerPaymentMethodVerifiedEventNotification.Event.OrderId);
            orderToUpdate.SetBuyerId(buyerPaymentMethodVerifiedEventNotification.Event.Buyer.Id);
            orderToUpdate.SetPaymentId(buyerPaymentMethodVerifiedEventNotification.Event.Payment.Id);                                                

            _logger.CreateLogger(nameof(UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler))
                .LogTrace($"Order with Id: {buyerPaymentMethodVerifiedEventNotification.Event.OrderId} has been successfully updated with a payment method id: { buyerPaymentMethodVerifiedEventNotification.Event.Payment.Id }");                        
        }
    }  
}
