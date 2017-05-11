using System.Collections.Generic;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderStockNotConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public List<ConfirmedOrderStockItem> OrderStockItems { get; }

        public OrderStockNotConfirmedIntegrationEvent(int orderId,
            List<ConfirmedOrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }

    public class ConfirmedOrderStockItem
    {
        public int ProductId { get; }
        public bool Confirmed { get; }

        public ConfirmedOrderStockItem(int productId, bool confirmed)
        {
            ProductId = productId;
            Confirmed = confirmed;
        }
    }
}