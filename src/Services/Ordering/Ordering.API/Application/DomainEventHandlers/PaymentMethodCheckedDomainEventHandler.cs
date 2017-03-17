using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;
using System;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers
{
    public class PaymentMethodCheckedDomainEventHandler : IAsyncNotificationHandler<PaymentMethodCheckedDomainEvent>
    {
        private readonly IOrderRepository<Order> _orderRepository;
        private readonly ILoggerFactory _logger;
        public PaymentMethodCheckedDomainEventHandler(IOrderRepository<Order> orderRepository, ILoggerFactory logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PaymentMethodCheckedDomainEvent paymentMethodNotification)
        {
            var orderToUpdate = await _orderRepository.GetAsync(paymentMethodNotification.OrderId);
            orderToUpdate.SetBuyerId(paymentMethodNotification.Buyer.Id);
            orderToUpdate.SetPaymentId(paymentMethodNotification.Payment.Id);
            
            await _orderRepository.UnitOfWork
                .SaveEntitiesAsync();
                                       
            _logger.CreateLogger(nameof(PaymentMethodCheckedDomainEventHandler))
                .LogTrace($"Order with Id: {paymentMethodNotification.OrderId} has been successfully updated with a new payment method id: { paymentMethodNotification.Payment.Id }");
        }
    }
}
