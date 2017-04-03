using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents
{
    public interface ICatalogIntegrationEventService
    {
        Task SaveEventAsync(IntegrationEvent evt);
        Task PublishAsync(IntegrationEvent evt);
    }
}
