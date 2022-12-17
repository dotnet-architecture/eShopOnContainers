using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Domain.Events;

namespace Ordering.API.Application.DomainEventHandlers.OrderCoupon;

public class OrderStatusChangedToAwaitingCouponValidationDomainEventHandler : INotificationHandler<OrderStatusChangedToAwaitingCouponValidationDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ILoggerFactory _logger;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderStatusChangedToAwaitingCouponValidationDomainEventHandler(
        IOrderRepository orderRepository,
        IBuyerRepository buyerRepository,
        ILoggerFactory logger,
        IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderStatusChangedToAwaitingCouponValidationDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.CreateLogger<OrderStatusChangedToAwaitingCouponValidationDomainEventHandler>()
            .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})", domainEvent.OrderId, nameof(OrderStatus.StockConfirmed), OrderStatus.StockConfirmed.Id);

        var order = await _orderRepository.GetAsync(domainEvent.OrderId);
        var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

        var integrationEvent = new OrderStatusChangedToAwaitingCouponValidationIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name, order.DiscountCode);

        await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}