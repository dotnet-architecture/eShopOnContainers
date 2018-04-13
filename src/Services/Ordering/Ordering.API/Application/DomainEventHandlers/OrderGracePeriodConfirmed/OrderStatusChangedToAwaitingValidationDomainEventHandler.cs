namespace Ordering.API.Application.DomainEventHandlers.OrderGracePeriodConfirmed
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
    using Microsoft.AspNetCore.Http;
    using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;

    public class OrderStatusChangedToAwaitingValidationDomainEventHandler
                   : INotificationHandler<OrderStatusChangedToAwaitingValidationDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public OrderStatusChangedToAwaitingValidationDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            IHubContext<NotificationsHub> hubContext)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _buyerRepository = buyerRepository;
            _orderingIntegrationEventService = orderingIntegrationEventService;
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task Handle(OrderStatusChangedToAwaitingValidationDomainEvent orderStatusChangedToAwaitingValidationDomainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger(nameof(OrderStatusChangedToAwaitingValidationDomainEvent))
                  .LogTrace($"Order with Id: {orderStatusChangedToAwaitingValidationDomainEvent.OrderId} has been successfully updated with " +
                            $"a status order id: {OrderStatus.AwaitingValidation.Id}");

            var order = await _orderRepository.GetAsync(orderStatusChangedToAwaitingValidationDomainEvent.OrderId);

            var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

            var orderStockList = orderStatusChangedToAwaitingValidationDomainEvent.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

            var orderStatusChangedToAwaitingValidationIntegrationEvent = new OrderStatusChangedToAwaitingValidationIntegrationEvent(
                orderStatusChangedToAwaitingValidationDomainEvent.OrderId, orderStockList);
            await _orderingIntegrationEventService.PublishThroughEventBusAsync(orderStatusChangedToAwaitingValidationIntegrationEvent);
           
            await _hubContext.Clients
                .Group(buyer.Name)
                .SendAsync("UpdatedOrderState", new { OrderId = order.Id, Status = order.OrderStatus.Name });
        }
    }  
}