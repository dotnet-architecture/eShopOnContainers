namespace SagaManager.IntegrationEvents
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using System;

    public class ConfirmGracePeriodEvent : IConfirmGracePeriodEvent
    {
        private readonly IEventBus _eventBus;

        public ConfirmGracePeriodEvent(IEventBus eventBus) => 
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

        public void PublishThroughEventBus(IntegrationEvent evt) => _eventBus.Publish(evt);
    }
}