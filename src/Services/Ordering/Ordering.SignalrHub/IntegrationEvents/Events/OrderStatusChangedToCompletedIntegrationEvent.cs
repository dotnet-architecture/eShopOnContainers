namespace Ordering.SignalrHub.IntegrationEvents.Events;

public record OrderStatusChangedToCompletedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; private init; }
    public string OrderStatus { get; private init; }
    public string BuyerName { get; private init; }

    public OrderStatusChangedToCompletedIntegrationEvent(int orderId, string orderStatus, string buyerName)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
    }
}
