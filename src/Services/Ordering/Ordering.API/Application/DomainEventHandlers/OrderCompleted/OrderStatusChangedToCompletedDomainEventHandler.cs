namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers.OrderCompleted;
    
public class OrderStatusChangedToCompletedDomainEventHandler
                : INotificationHandler<OrderStatusChangedToCompletedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILoggerFactory _logger;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;


    public OrderStatusChangedToCompletedDomainEventHandler(
        IOrderRepository orderRepository, ILoggerFactory logger,
        IBuyerRepository buyerRepository,
        IOrderingIntegrationEventService orderingIntegrationEventService
        )
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
    }

    public async Task Handle(OrderStatusChangedToCompletedDomainEvent orderStatusChangedToCompletedDomainEvent, CancellationToken cancellationToken)
    {
        _logger.CreateLogger<OrderStatusChangedToCompletedDomainEventHandler>()
            .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
                orderStatusChangedToCompletedDomainEvent.OrderId, nameof(OrderStatus.Complete), OrderStatus.Complete.Id);

        var order = await _orderRepository.GetAsync(orderStatusChangedToCompletedDomainEvent.OrderId);
        var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

        var orderStockList = orderStatusChangedToCompletedDomainEvent.OrderItems
            .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

        var orderStatusChangedToCompletedIntegrationEvent = new OrderStatusChangedToCompletedIntegrationEvent(
            orderStatusChangedToCompletedDomainEvent.OrderId,
            order.OrderStatus.Name,
            buyer.Name,
            orderStockList);

        await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToCompletedIntegrationEvent);
    }
}
