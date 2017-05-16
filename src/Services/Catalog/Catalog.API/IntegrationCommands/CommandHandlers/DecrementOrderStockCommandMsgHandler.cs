namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationCommands.CommandHandlers
{
    using BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Infrastructure;
    using global::Catalog.API.Infrastructure.Exceptions;
    using global::Catalog.API.IntegrationEvents;
    using Model;
    using Commands;

    public class DecrementOrderStockCommandMsgHandler : IIntegrationEventHandler<DecrementOrderStockCommandMsg>
    {
        private readonly CatalogContext _catalogContext;

        public DecrementOrderStockCommandMsgHandler(CatalogContext catalogContext)
        {
            _catalogContext = catalogContext;
        }

        public async Task Handle(DecrementOrderStockCommandMsg @event)
        {
            //we're not blocking stock/inventory
            foreach (var orderStockItem in @event.OrderStockItems)
            {
                var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);

                catalogItem.RemoveStock(orderStockItem.Units);
            }

            await _catalogContext.SaveChangesAsync();
        }
    }
}