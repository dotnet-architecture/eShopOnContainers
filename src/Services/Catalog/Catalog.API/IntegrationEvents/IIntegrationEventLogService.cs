using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents
{
    public interface IIntegrationEventLogService
    {
        Task SaveEventAsync(IntegrationEvent @event);
        Task MarkEventAsPublishedAsync(IntegrationEvent @event);
    }
}
