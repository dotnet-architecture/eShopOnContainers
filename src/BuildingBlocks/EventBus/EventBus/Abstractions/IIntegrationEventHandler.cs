using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler 
        where TIntegrationEvent: IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
