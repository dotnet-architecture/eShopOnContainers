namespace SagaManager.IntegrationEvents
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public interface ISagaManagerIntegrationEventService
    {
        void PublishThroughEventBus(IntegrationEvent evt);
    }
}