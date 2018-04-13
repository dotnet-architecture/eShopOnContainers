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
    using Microsoft.AspNetCore.SignalR;
    using Ordering.API.Infrastructure.Hubs;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;

    public class OrderStatusChangedToPaidDomainEventHandler
                   : INotificationHandler<OrderStatusChangedToPaidDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public OrderStatusChangedToPaidDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            IHubContext<NotificationsHub> hubContext)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task Handle(OrderStatusChangedToPaidDomainEvent orderStatusChangedToPaidDomainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger(nameof(OrderStatusChangedToPaidDomainEventHandler))
             .LogTrace($"Order with Id: {orderStatusChangedToPaidDomainEvent.OrderId} has been successfully updated with " +
                       $"a status order id: {OrderStatus.Paid.Id}");

            var order = await _orderRepository.GetAsync(orderStatusChangedToPaidDomainEvent.OrderId);
            var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

            var orderStockList = orderStatusChangedToPaidDomainEvent.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

            var orderStatusChangedToPaidIntegrationEvent = new OrderStatusChangedToPaidIntegrationEvent(orderStatusChangedToPaidDomainEvent.OrderId,
                orderStockList);
            await _orderingIntegrationEventService.PublishThroughEventBusAsync(orderStatusChangedToPaidIntegrationEvent);

            await _hubContext.Clients
                .Group(buyer.Name)
                .SendAsync("UpdatedOrderState", new { OrderId = order.Id, Status = order.OrderStatus.Name });
        }
    }  
}