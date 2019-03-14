namespace Payment.API.IntegrationEvents.Events
{
    public class OrderStatusChangedToStockConfirmedIntegrationEvent 
    {
        public int OrderId { get; }

        public OrderStatusChangedToStockConfirmedIntegrationEvent(int orderId)
            => OrderId = orderId;
    }
}