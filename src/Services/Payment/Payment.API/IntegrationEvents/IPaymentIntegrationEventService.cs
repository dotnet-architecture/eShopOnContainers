namespace Payment.API.IntegrationEvents
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public interface IPaymentIntegrationEventService
    {
        void PublishThroughEventBus(IntegrationEvent evt);
    }
}
