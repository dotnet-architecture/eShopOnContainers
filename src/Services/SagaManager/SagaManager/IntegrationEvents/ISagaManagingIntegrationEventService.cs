namespace SagaManager.IntegrationEvents
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public interface ISagaManagingIntegrationEventService
    {
        void PublishThroughEventBus(IntegrationEvent evt);
    }
}