using DotNetCore.CAP;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.EventHandling
{
    using System.Threading.Tasks;
    using Infrastructure;
    using System.Collections.Generic;
    using System.Linq;
    using Events;
    using Serilog.Context;
    using Extensions.Logging;

    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : ICapSubscribe
    {
        private readonly CatalogContext _catalogContext;
        private readonly ICapPublisher _eventBus;
        private readonly ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> _logger;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(
            CatalogContext catalogContext,
            ICapPublisher eventBus,
            ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger)
        {
            _catalogContext = catalogContext;
            _eventBus = eventBus;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [CapSubscribe(nameof(OrderStatusChangedToAwaitingValidationIntegrationEvent))]
        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{Program.AppName}"))
            {
                var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

                foreach (var orderStockItem in @event.OrderStockItems)
                {
                    var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);
                    var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
                    var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, hasStock);

                    confirmedOrderStockItems.Add(confirmedOrderStockItem);
                } 

                if (confirmedOrderStockItems.Any(c => !c.HasStock))
                {
                    await _eventBus.PublishAsync(nameof(OrderStockRejectedIntegrationEvent),
                        new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems));
                }
                else
                {
                    await _eventBus.PublishAsync(nameof(OrderStockConfirmedIntegrationEvent),
                        new OrderStockConfirmedIntegrationEvent(@event.OrderId));
                } 
            }
        }
    }
}