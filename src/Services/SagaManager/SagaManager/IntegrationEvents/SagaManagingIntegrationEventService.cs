namespace SagaManager.IntegrationEvents
{
    using System.Data.Common;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF.Services;
    using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
    using System;

    public class SagaManagingIntegrationEventService : ISagaManagingIntegrationEventService
    {
        private readonly IEventBus _eventBus;

        public SagaManagingIntegrationEventService(IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public void PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            _eventBus.Publish(evt);
        }
    }
}