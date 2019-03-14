namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class GracePeriodConfirmedIntegrationEvent 
    {
        public int OrderId { get; }

        public GracePeriodConfirmedIntegrationEvent(int orderId) =>
            OrderId = orderId;
    }
}
