using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<T, TH>(String vHost)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void SubscribeDynamic<TH>(string eventName, String vHost)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName, String vHost)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>(String vHost)
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}
