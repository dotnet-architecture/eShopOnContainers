using System.Collections.Generic;
using System.Linq;
using Catalog.API.Infrastructure.Exceptions;
using Catalog.API.IntegrationEvents;
using Microsoft.eShopOnContainers.Services.Catalog.API.Model;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.EventHandling
{
    using BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using BuildingBlocks.EventBus.Events;
    using Infrastructure;

    using Events;

    public class ConfirmOrderStockIntegrationEventHandler : IIntegrationEventHandler<ConfirmOrderStockIntegrationEvent>
    {
        private readonly CatalogContext _catalogContext;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

        public ConfirmOrderStockIntegrationEventHandler(CatalogContext catalogContext,
            ICatalogIntegrationEventService catalogIntegrationEventService)
        {
            _catalogContext = catalogContext;
            _catalogIntegrationEventService = catalogIntegrationEventService;
        }

        public async Task Handle(ConfirmOrderStockIntegrationEvent @event)
        {
            var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

            foreach (var orderStockItem in @event.OrderStockItems)
            {
                var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);
                CheckValidcatalogItemId(catalogItem);

                var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, 
                    catalogItem.AvailableStock >= orderStockItem.Units);

                confirmedOrderStockItems.Add(confirmedOrderStockItem);
            }

            //Create Integration Event to be published through the Event Bus
            var integrationEvent = confirmedOrderStockItems.Any(c => !c.Confirmed)
                ? (IntegrationEvent) new OrderStockNotConfirmedIntegrationEvent(@event.OrderId, confirmedOrderStockItems)
                : new OrderStockConfirmedIntegrationEvent(@event.OrderId);
          
            // Publish through the Event Bus and mark the saved event as published
            await _catalogIntegrationEventService.PublishThroughEventBusAsync(integrationEvent);
        }

        private void CheckValidcatalogItemId(CatalogItem catalogItem)
        {
            if (catalogItem is null)
            {
                throw new CatalogDomainException("Not able to process catalog event. Reason: no valid catalogItemId");
            }
        }
    }
}