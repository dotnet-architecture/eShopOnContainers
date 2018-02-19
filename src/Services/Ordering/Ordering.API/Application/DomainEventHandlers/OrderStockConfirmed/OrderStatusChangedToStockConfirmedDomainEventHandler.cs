using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;

namespace Ordering.API.Application.DomainEventHandlers.OrderStockConfirmed
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Microsoft.Extensions.Logging;
    using Domain.Events;
    using System;
    using System.Threading.Tasks;
    using Ordering.API.Application.IntegrationEvents;
    using Ordering.API.Application.IntegrationEvents.Events;
    using System.Threading;

    public class OrderStatusChangedToStockConfirmedDomainEventHandler
                   : INotificationHandler<DomainEventNotification<OrderStatusChangedToStockConfirmedDomainEvent>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToStockConfirmedDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(DomainEventNotification<OrderStatusChangedToStockConfirmedDomainEvent> orderStatusChangedToStockConfirmedDomainEventNotification, CancellationToken cancellationToken)
        {
            _logger.CreateLogger(nameof(OrderStatusChangedToStockConfirmedDomainEventHandler))
                .LogTrace($"Order with Id: {orderStatusChangedToStockConfirmedDomainEventNotification.Event.OrderId} has been successfully updated with " +
                          $"a status order id: {OrderStatus.StockConfirmed.Id}");

            var orderStatusChangedToStockConfirmedIntegrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(orderStatusChangedToStockConfirmedDomainEventNotification.Event.OrderId);
            await _orderingIntegrationEventService.PublishThroughEventBusAsync(orderStatusChangedToStockConfirmedIntegrationEvent);
        }
    }  
}