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
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
    using Microsoft.AspNetCore.SignalR;
    using Ordering.API.Infrastructure.Hubs;

    public class OrderStatusChangedToStockConfirmedDomainEventHandler
                   : INotificationHandler<OrderStatusChangedToStockConfirmedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly ILoggerFactory _logger;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToStockConfirmedDomainEventHandler(
            IOrderRepository orderRepository, 
            IBuyerRepository buyerRepository,
            ILoggerFactory logger,
            IHubContext<NotificationsHub> hubContext,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStatusChangedToStockConfirmedDomainEvent orderStatusChangedToStockConfirmedDomainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger(nameof(OrderStatusChangedToStockConfirmedDomainEventHandler))
                .LogTrace($"Order with Id: {orderStatusChangedToStockConfirmedDomainEvent.OrderId} has been successfully updated with " +
                          $"a status order id: {OrderStatus.StockConfirmed.Id}");

            var order = await _orderRepository.GetAsync(orderStatusChangedToStockConfirmedDomainEvent.OrderId);
            var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

            var orderStatusChangedToStockConfirmedIntegrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(orderStatusChangedToStockConfirmedDomainEvent.OrderId);
            await _orderingIntegrationEventService.PublishThroughEventBusAsync(orderStatusChangedToStockConfirmedIntegrationEvent);

            await _hubContext.Clients
                .Group(buyer.Name)
                .SendAsync("UpdatedOrderState", new { OrderId = order.Id, Status = order.OrderStatus.Name });
        }
    }  
}