using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;

namespace Ordering.API.Application.DomainEventHandlers.OrderPaid
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Microsoft.Extensions.Logging;
    using Domain.Events;
    using System;
    using System.Threading.Tasks;
    using Ordering.API.Application.IntegrationEvents;
    using System.Linq;
    using Ordering.API.Application.IntegrationEvents.Events;
    using System.Threading;

    public class OrderStatusChangedToPaidDomainEventHandler
                   : INotificationHandler<DomainEventNotification<OrderStatusChangedToPaidDomainEvent>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToPaidDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(DomainEventNotification<OrderStatusChangedToPaidDomainEvent> orderStatusChangedToPaidDomainEventNotification, CancellationToken cancellationToken)
        {
            _logger.CreateLogger(nameof(OrderStatusChangedToPaidDomainEventHandler))
             .LogTrace($"Order with Id: {orderStatusChangedToPaidDomainEventNotification.Event.OrderId} has been successfully updated with " +
                       $"a status order id: {OrderStatus.Paid.Id}");

            var orderStockList = orderStatusChangedToPaidDomainEventNotification.Event.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

            var orderStatusChangedToPaidIntegrationEvent = new OrderStatusChangedToPaidIntegrationEvent(orderStatusChangedToPaidDomainEventNotification.Event.OrderId,
                orderStockList);
            await _orderingIntegrationEventService.PublishThroughEventBusAsync(orderStatusChangedToPaidIntegrationEvent);
        }
    }  
}