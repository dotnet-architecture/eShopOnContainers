using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.IntegrationEvents
{
    public interface IOrderingIntegrationEventService
    {
        Task SaveEventAsync(IntegrationEvent evt);
        Task PublishAsync(IntegrationEvent evt);
    }
}
