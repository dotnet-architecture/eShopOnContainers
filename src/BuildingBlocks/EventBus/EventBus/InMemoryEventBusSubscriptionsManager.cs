using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus
{
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {


        //private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly Dictionary<CompositeHandler, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public event EventHandler<string> OnEventRemoved;

        public InMemoryEventBusSubscriptionsManager()
        {
            //_handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _handlers = new Dictionary<CompositeHandler, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();
        public void Clear() => _handlers.Clear();

        public void AddDynamicSubscription<TH>(string eventName, String vHost)
            where TH : IDynamicIntegrationEventHandler
        {
            DoAddSubscription(typeof(TH), eventName, isDynamic: true, vHost);
        }

        public void AddSubscription<T, TH>(String vHost)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();

            DoAddSubscription(typeof(TH), eventName, isDynamic: false, vHost);
            
            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }
        }

        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic, String vHost)
        {
            var compositeHandler = new CompositeHandler{TenantVHostName = vHost, EventName = eventName};

            if (!HasSubscriptionsForEvent(eventName, vHost))
            {
                _handlers.Add(compositeHandler, new List<SubscriptionInfo>());
            }

            if (_handlers[compositeHandler].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            if (isDynamic)
            {
                _handlers[compositeHandler].Add(SubscriptionInfo.Dynamic(handlerType));
            }
            else
            {
                _handlers[compositeHandler].Add(SubscriptionInfo.Typed(handlerType));
            }
        }


        public void RemoveDynamicSubscription<TH>(string eventName, String vHost)
            where TH : IDynamicIntegrationEventHandler
        {
            var handlerToRemove = FindDynamicSubscriptionToRemove<TH>(eventName, vHost);
            DoRemoveHandler(eventName, handlerToRemove, vHost);
        }


        public void RemoveSubscription<T, TH>(String vHost)
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            var handlerToRemove = FindSubscriptionToRemove<T, TH>(vHost);
            var eventName = GetEventKey<T>();
            DoRemoveHandler(eventName, handlerToRemove, vHost);
        }


        private void DoRemoveHandler(string eventName, SubscriptionInfo subsToRemove, String vHost)
        {
            if (subsToRemove != null)
            {
                
                var compositeHandler = new CompositeHandler{EventName = eventName, TenantVHostName = vHost};
                _handlers[compositeHandler].Remove(subsToRemove);
                if (!_handlers[compositeHandler].Any())
                {
                    _handlers.Remove(compositeHandler);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                    }
                    RaiseOnEventRemoved(eventName);
                }

            }
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>(String vHost) where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key, vHost);
        }
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName, String vHost) => _handlers[new CompositeHandler{EventName = eventName, TenantVHostName = vHost}];

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }


        private SubscriptionInfo FindDynamicSubscriptionToRemove<TH>(string eventName, String vHost)
            where TH : IDynamicIntegrationEventHandler
        {
            return DoFindSubscriptionToRemove(eventName, typeof(TH), vHost);
        }


        private SubscriptionInfo FindSubscriptionToRemove<T, TH>(String vHost)
             where T : IntegrationEvent
             where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();

            return DoFindSubscriptionToRemove(eventName, typeof(TH), vHost);
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType, String vHost)
        {
            
            if (!HasSubscriptionsForEvent(eventName, vHost))
            {
                return null;
            }
            
            var compositeHandler = new CompositeHandler{EventName = eventName, TenantVHostName = vHost};

            return _handlers[compositeHandler].SingleOrDefault(s => s.HandlerType == handlerType);

        }

        public bool HasSubscriptionsForEvent<T>(String vHost) where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            
            return HasSubscriptionsForEvent(key, vHost);
        }
        public bool HasSubscriptionsForEvent(string eventName, String vHost) => _handlers.ContainsKey(new CompositeHandler{EventName = eventName, TenantVHostName = vHost});

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }
    }
}


class CompositeHandler
{
    public String TenantVHostName { get; set; }
    public String EventName { get; set; }
}