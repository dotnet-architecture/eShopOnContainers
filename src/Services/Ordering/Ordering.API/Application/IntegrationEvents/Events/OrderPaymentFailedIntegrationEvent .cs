namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent 
    {
        public int OrderId { get; }

        public OrderPaymentFailedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}