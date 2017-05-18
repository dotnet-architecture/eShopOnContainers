namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationCommands.CommandHandlers
{
    using BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using BuildingBlocks.EventBus.Events;
    using Infrastructure;
    using System.Collections.Generic;
    using System.Linq;
    using global::Catalog.API.Infrastructure.Exceptions;
    using global::Catalog.API.IntegrationEvents;
    using Model;
    using Commands;
    using IntegrationEvents.Events;

    public class ConfirmOrderStockCommandHandler : IIntegrationEventHandler<ConfirmOrderStockCommand>
    {
        private readonly CatalogContext _catalogContext;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

        public ConfirmOrderStockCommandHandler(CatalogContext catalogContext,
            ICatalogIntegrationEventService catalogIntegrationEventService)
        {
            _catalogContext = catalogContext;
            _catalogIntegrationEventService = catalogIntegrationEventService;
        }

        public async Task Handle(ConfirmOrderStockCommand command)
        {
            var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

            foreach (var orderStockItem in command.OrderStockItems)
            {
                var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);
                CheckValidcatalogItemId(catalogItem);

                var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, 
                    catalogItem.AvailableStock >= orderStockItem.Units);

                confirmedOrderStockItems.Add(confirmedOrderStockItem);
            }

            var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
                ? (IntegrationEvent) new OrderStockNotConfirmedIntegrationEvent(command.OrderId, confirmedOrderStockItems)
                : new OrderStockConfirmedIntegrationEvent(command.OrderId);

            await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(confirmedIntegrationEvent);
            await _catalogIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);
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