namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Events;
  
/// <summary>
/// Event used when the order is paid
/// </summary>
public class OrderStatusChangedToPaidDomainEvent
    : INotification
{
    public int OrderId { get; }
    public IEnumerable<OrderItem> OrderItems { get; }

    public OrderStatusChangedToPaidDomainEvent(int orderId,
        IEnumerable<OrderItem> orderItems)
    {
        OrderId = orderId;
        OrderItems = orderItems;
    }
}
