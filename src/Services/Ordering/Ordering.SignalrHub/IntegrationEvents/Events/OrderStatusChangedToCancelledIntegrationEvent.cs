namespace Ordering.SignalrHub.IntegrationEvents.Events
{
    public class OrderStatusChangedToCancelledIntegrationEvent 
    {
        public int OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }

        public OrderStatusChangedToCancelledIntegrationEvent(int orderId, string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
