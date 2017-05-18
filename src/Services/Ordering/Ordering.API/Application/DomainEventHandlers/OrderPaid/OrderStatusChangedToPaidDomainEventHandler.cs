namespace Ordering.API.Application.DomainEventHandlers.OrderPaid
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Microsoft.Extensions.Logging;
    using Domain.Events;
    using System;
    using System.Threading.Tasks;
    using Ordering.API.Application.IntegrationCommands.Commands;
    using Ordering.API.Application.IntegrationEvents;
    using System.Linq;

    public class OrderStatusChangedToPaidDomainEventHandler
                   : IAsyncNotificationHandler<OrderStatusChangedToPaidDomainEvent>
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

        public async Task Handle(OrderStatusChangedToPaidDomainEvent orderStatusChangedToPaidDomainEvent)
        {
            _logger.CreateLogger(nameof(OrderStatusChangedToPaidDomainEventHandler))
                .LogTrace($"Order with Id: {orderStatusChangedToPaidDomainEvent.OrderId} has been successfully updated with " +
                          $"a status order id: {OrderStatus.Paid.Id}");

            var orderStockList = orderStatusChangedToPaidDomainEvent.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

            var decrementOrderStockCommand = new DecrementOrderStockCommand(orderStatusChangedToPaidDomainEvent.OrderId,
                orderStockList);
            await _orderingIntegrationEventService.SaveEventAndOrderingContextChangesAsync(decrementOrderStockCommand);
            await _orderingIntegrationEventService.PublishThroughEventBusAsync(decrementOrderStockCommand);
        }
    }  
}