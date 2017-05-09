using Catalog.API.IntegrationEvents;

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
            IntegrationEvent integrationEvent = new OrderStockConfirmedIntegrationEvent(@event.OrderId);

            //TODO: Check the stock products units

            await _catalogIntegrationEventService.PublishThroughEventBusAsync(integrationEvent);
        }
    }
}