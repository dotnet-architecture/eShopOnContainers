namespace Ordering.BackgroundTasks.IntegrationEvents
{
    public class GracePeriodConfirmedIntegrationEvent 
    {
        public int OrderId { get; }

        public GracePeriodConfirmedIntegrationEvent(int orderId) =>
            OrderId = orderId;
    }
}
