using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using static Microsoft.eShopOnContainers.BuildingBlocks.EventBus.InMemoryEventBusSubscriptionsManager;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddDynamicSubscription<TH>(string eventName, String vHost)
           where TH : IDynamicIntegrationEventHandler;

        void AddSubscription<T, TH>(String vHost)
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>(String vHost)
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent;
        void RemoveDynamicSubscription<TH>(string eventName, String vHost)
            where TH : IDynamicIntegrationEventHandler;

        bool HasSubscriptionsForEvent<T>(String vHost) where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName, String vHost);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>(String vHost) where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName, String vHost);
        string GetEventKey<T>();
    }
}