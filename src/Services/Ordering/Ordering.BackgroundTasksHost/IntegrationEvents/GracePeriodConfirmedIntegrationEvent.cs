using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.BackgroundTasksHost.IntegrationEvents
{
    public class GracePeriodConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public GracePeriodConfirmedIntegrationEvent(int orderId) =>
            OrderId = orderId;
    }
}
