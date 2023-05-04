namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers;

public class OrderShippedDomainEventHandler
                : INotificationHandler<OrderShippedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
    private readonly ILogger _logger;

    public OrderShippedDomainEventHandler(
        IOrderRepository orderRepository,
        ILogger<OrderShippedDomainEventHandler> logger,
        IBuyerRepository buyerRepository,
        IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderShippedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        OrderingApiTrace.LogOrderStatusUpdated(_logger, domainEvent.Order.Id, nameof(OrderStatus.Shipped), OrderStatus.Shipped.Id);

        var order = await _orderRepository.GetAsync(domainEvent.Order.Id);
        var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

        var integrationEvent = new OrderStatusChangedToShippedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
        await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
