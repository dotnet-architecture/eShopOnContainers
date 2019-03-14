namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class OrderPaymentSuccededIntegrationEvent 
    {
        public int OrderId { get; }

        public OrderPaymentSuccededIntegrationEvent(int orderId) => OrderId = orderId;
    }
}