namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class OrderStockConfirmedIntegrationEvent
    {
        public int OrderId { get; }

        public OrderStockConfirmedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}