using Microsoft.eShopOnContainers.Services.Ordering.Domain.Events;
using Ordering.API.Application.IntegrationEvents.Events;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    public class OrderCouponConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderCouponConfirmedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly IEventBus _eventBus;

        public OrderCouponConfirmedIntegrationEventHandler(
            IOrderRepository orderRepository,
            IBuyerRepository buyerRepository,
            ILoggerFactory logger,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            IEventBus eventBus)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingIntegrationEventService = orderingIntegrationEventService;
            _eventBus = eventBus ?? throw new ArgumentNullException();
        }

        public async Task Handle(OrderCouponConfirmedIntegrationEvent @event)
        {
            // Add new statuses here
            //_logger.CreateLogger<OrderStatusChangedToStockConfirmedDomainEventHandler>()
            //.LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
            //    orderStatusChangedToStockConfirmedDomainEvent.OrderId, nameof(OrderStatus.StockConfirmed), OrderStatus.StockConfirmed.Id);

            var order = await _orderRepository.GetAsync(@event.OrderId);
            var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());



            var orderStatusChangedToStockConfirmedIntegrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
            _eventBus.Publish(orderStatusChangedToStockConfirmedIntegrationEvent);
            //fix it
            //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToStockConfirmedIntegrationEvent);
        }
    }
}
