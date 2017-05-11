using System.Linq;

namespace Ordering.API.Application.DomainEventHandlers.OrderStartedEvent
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Microsoft.Extensions.Logging;
    using Domain.Events;
    using System;
    using System.Threading.Tasks;
    using Ordering.API.Application.IntegrationCommands.Commands;
    using Ordering.API.Application.IntegrationEvents;

    public class OrderStatusChangedWhenOrderStockConfirmedDomainEventHandler
                   : IAsyncNotificationHandler<OrderStockConfirmedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedWhenOrderStockConfirmedDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStockConfirmedDomainEvent orderStockMethodVerifiedDomainEvent)
        {
            _logger.CreateLogger(nameof(OrderStatusChangedWhenOrderStockConfirmedDomainEventHandler))
                .LogTrace($"Order with Id: {orderStockMethodVerifiedDomainEvent.OrderId} has been successfully updated with " +
                          $"a status order id: { orderStockMethodVerifiedDomainEvent.OrderStatus.Id }");

            if (orderStockMethodVerifiedDomainEvent.OrderStatus == OrderStatus.StockValidated)
            {
                var payOrderCommandMsg = new PayOrderCommandMsg(orderStockMethodVerifiedDomainEvent.OrderId);
                await _orderingIntegrationEventService.SaveEventAndOrderingContextChangesAsync(payOrderCommandMsg);
                await _orderingIntegrationEventService.PublishThroughEventBusAsync(payOrderCommandMsg);
            }
        }
    }  
}