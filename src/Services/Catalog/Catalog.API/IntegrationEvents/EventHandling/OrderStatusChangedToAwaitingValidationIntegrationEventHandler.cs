namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.EventHandling
{
    using BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using BuildingBlocks.EventBus.Events;
    using Infrastructure;
    using System.Collections.Generic;
    using System.Linq;
    using global::Catalog.API.IntegrationEvents;
    using IntegrationEvents.Events;

    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : 
        IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
    {
        private readonly CatalogContext _catalogContext;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(CatalogContext catalogContext,
            ICatalogIntegrationEventService catalogIntegrationEventService)
        {
            _catalogContext = catalogContext;
            _catalogIntegrationEventService = catalogIntegrationEventService;
        }

        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent command)
        {
            var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

            foreach (var orderStockItem in command.OrderStockItems)
            {
                var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);
                var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
                var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, hasStock);

                confirmedOrderStockItems.Add(confirmedOrderStockItem);
            }

            var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
                ? (IntegrationEvent) new OrderStockRejectedIntegrationEvent(command.OrderId, confirmedOrderStockItems)
                : new OrderStockConfirmedIntegrationEvent(command.OrderId);

            await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(confirmedIntegrationEvent);
            await _catalogIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);
        }
    }
}