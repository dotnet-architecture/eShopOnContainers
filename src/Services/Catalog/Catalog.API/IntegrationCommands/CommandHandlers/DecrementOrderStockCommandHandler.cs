namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationCommands.CommandHandlers
{
    using BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Infrastructure;
    using Commands;

    public class DecrementOrderStockCommandHandler : IIntegrationEventHandler<DecrementOrderStockCommand>
    {
        private readonly CatalogContext _catalogContext;

        public DecrementOrderStockCommandHandler(CatalogContext catalogContext)
        {
            _catalogContext = catalogContext;
        }

        public async Task Handle(DecrementOrderStockCommand command)
        {
            //we're not blocking stock/inventory
            foreach (var orderStockItem in command.OrderStockItems)
            {
                var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);

                catalogItem.RemoveStock(orderStockItem.Units);
            }

            await _catalogContext.SaveChangesAsync();
        }
    }
}