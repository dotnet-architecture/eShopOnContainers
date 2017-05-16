namespace SagaManager.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ConfirmGracePeriodCommandMsg : IntegrationEvent
    {
        public int OrderId { get;}

        public ConfirmGracePeriodCommandMsg(int orderId) => OrderId = orderId;
    }
}