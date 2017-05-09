using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events
{
    using BuildingBlocks.EventBus.Events;

    public class OrderStockNotConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public IEnumerable<ConfirmedOrderStockItem> OrderStockItem { get; }

        public OrderStockNotConfirmedIntegrationEvent(int orderId, 
            IEnumerable<ConfirmedOrderStockItem> orderStockItem)
        {
            OrderId = orderId;
            OrderStockItem = orderStockItem;
        }

        public class ConfirmedOrderStockItem
        {
            public int ProductId { get; }
            public int Units { get; }
            public bool Confirmed { get; }

            public ConfirmedOrderStockItem(int productId, int units, bool confirmed)
            {
                ProductId = productId;
                Units = units;
                Confirmed = confirmed;
            }
        }
    }
}