using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<Delegate>> _handlers;
        private readonly List<Type> _eventTypes;

        public event EventHandler<string> OnEventRemoved;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<Delegate>>();
            _eventTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();
        public void Clear() => _handlers.Clear();

        public void AddSubscription<T, TH>(Func<TH> handler) 
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var key = GetEventKey<T>();
            if (!HasSubscriptionsForEvent<T>())
            {
                _handlers.Add(key, new List<Delegate>());
            }
            _handlers[key].Add(handler);
            _eventTypes.Add(typeof(T));
        }

        public void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            var handlerToRemove = FindHandlerToRemove<T, TH>();
            if (handlerToRemove != null)
            {
                var key = GetEventKey<T>();
                _handlers[key].Remove(handlerToRemove);
                if (!_handlers[key].Any())
                {
                    _handlers.Remove(key);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == key);
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                        RaiseOnEventRemoved(eventType.Name);
                    }
                }
                
            }
        }

        public IEnumerable<Delegate> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }
        public IEnumerable<Delegate> GetHandlersForEvent(string eventName) => _handlers[eventName];

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            if (handler != null)
            {
                OnEventRemoved(this, eventName);
            }
        }

        private Delegate FindHandlerToRemove<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            if (!HasSubscriptionsForEvent<T>())
            {
                return null;
            }

            var key = GetEventKey<T>();
            foreach (var func in _handlers[key])
            {
                var genericArgs = func.GetType().GetGenericArguments();
                if (genericArgs.SingleOrDefault() == typeof(TH))
                {
                    return func;
                }
            }

            return null;
        }

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }
        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);
       
        public Type GetEventTypeByName(string eventName) => _eventTypes.Single(t => t.Name == eventName);

        private string GetEventKey<T>()
        {
            return typeof(T).Name;
        }
    }
}
