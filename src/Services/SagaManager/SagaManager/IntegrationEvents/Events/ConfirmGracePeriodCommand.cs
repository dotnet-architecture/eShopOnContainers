namespace SagaManager.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ConfirmGracePeriodCommand : IntegrationEvent
    {
        public int OrderId { get;}

        public ConfirmGracePeriodCommand(int orderId) => OrderId = orderId;
    }
}