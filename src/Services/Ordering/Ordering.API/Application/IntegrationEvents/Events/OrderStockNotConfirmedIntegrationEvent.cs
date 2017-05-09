using System.Collections.Generic;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

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