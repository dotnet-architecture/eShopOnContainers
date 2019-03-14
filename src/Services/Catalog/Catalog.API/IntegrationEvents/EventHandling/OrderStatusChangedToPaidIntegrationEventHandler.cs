using DotNetCore.CAP;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.EventHandling
{
    using System.Threading.Tasks;
    using Infrastructure;
    using Events;
    using Extensions.Logging;
    using Serilog.Context;

    //OrderStatusChangedToPaidIntegrationEvent
    public class OrderStatusChangedToPaidIntegrationEventHandler : ICapSubscribe
    {
        private readonly CatalogContext _catalogContext;
        private readonly ILogger<OrderStatusChangedToPaidIntegrationEventHandler> _logger;

        public OrderStatusChangedToPaidIntegrationEventHandler(
            CatalogContext catalogContext,
            ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger)
        {
            _catalogContext = catalogContext;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [CapSubscribe(nameof(OrderStatusChangedToPaidIntegrationEvent))]
        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {AppName} - ({@IntegrationEvent})", Program.AppName, @event);

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
}