namespace SagaManager.IntegrationEvents
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public interface IConfirmGracePeriodEvent
    {
        void PublishThroughEventBus(IntegrationEvent evt);
    }
}