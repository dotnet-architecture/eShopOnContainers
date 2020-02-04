using System;
using System.Collections.Generic;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ
{
    public class MultiEventBusRabbitMQ : IMultiEventBus
    {
        private List<IEventBus> _eventBuses;

        public MultiEventBusRabbitMQ(List<IEventBus> eventBuses)
        {
            _eventBuses = eventBuses;
        }

        public void AddEventBus(IEventBus eventBus)
        {
            _eventBuses.Add(eventBus);
        }

        public void Publish(IntegrationEvent @event)
        {
            //TODO
            var actualEventBus = _eventBuses.Find(e => e.GetVHost().Equals("TenantA"));

            if (actualEventBus == null)
            {
                throw new Exception();
            }

            actualEventBus.Publish(@event);
        }

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            _eventBuses.ForEach(e => { e.Subscribe<T, TH>(); });
        }
    }
}