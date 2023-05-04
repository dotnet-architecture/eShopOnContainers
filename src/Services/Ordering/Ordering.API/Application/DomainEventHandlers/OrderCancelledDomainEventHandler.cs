namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers;

public partial class OrderCancelledDomainEventHandler
                : INotificationHandler<OrderCancelledDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ILogger _logger;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderCancelledDomainEventHandler(
        IOrderRepository orderRepository,
        ILogger<OrderCancelledDomainEventHandler> logger,
        IBuyerRepository buyerRepository,
        IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        OrderingApiTrace.LogOrderStatusUpdated(_logger, domainEvent.Order.Id, nameof(OrderStatus.Cancelled), OrderStatus.Cancelled.Id);

        var order = await _orderRepository.GetAsync(domainEvent.Order.Id);
        var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

        var integrationEvent = new OrderStatusChangedToCancelledIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
        await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
