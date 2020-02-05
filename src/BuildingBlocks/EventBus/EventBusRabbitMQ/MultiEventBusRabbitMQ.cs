using System;
using System.Collections.Generic;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ
{
    public class MultiEventBusRabbitMQ : IMultiEventBus
    {
        private List<IEventBus> _eventBuses;
        private Dictionary<int, String> _tenants;

        public MultiEventBusRabbitMQ(List<IEventBus> eventBuses, Dictionary<int, String> tenants)
        {
            _eventBuses = eventBuses;
            _tenants = tenants;
        }

        public void AddEventBus(IEventBus eventBus)
        {
            _eventBuses.Add(eventBus);
        }

        public void Publish(IntegrationEvent @event)
        {
            if (@event.TenantId == 0)//System wide event?
            {
                _eventBuses.ForEach(eventBus =>
                {
                    eventBus.Publish(@event);
                });
            }
            
            //TODO requires ALL events to have tenantId set!
            _tenants.TryGetValue(@event.TenantId, out String tenantName);
            var actualEventBus = _eventBuses.Find(e => e.GetVHost().Equals(tenantName));

            actualEventBus.Publish(@event);
        }

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            _eventBuses.ForEach(e => { e.Subscribe<T, TH>(); });
        }
    }
}