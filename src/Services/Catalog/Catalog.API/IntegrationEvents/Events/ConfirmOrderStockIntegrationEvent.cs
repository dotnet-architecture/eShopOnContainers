namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events
{
    using BuildingBlocks.EventBus.Events;
    using System.Collections.Generic;

    public class ConfirmOrderStockIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public IEnumerable<OrderStockItem> OrderStockItem { get; }

        public ConfirmOrderStockIntegrationEvent(int orderId,
            IEnumerable<OrderStockItem> orderStockItem)
        {
            OrderId = orderId;
            OrderStockItem = orderStockItem;
        }
    }

    public class OrderStockItem
    {
        public int ProductId { get; }
        public int Units { get; }

        public OrderStockItem(int productId, int units)
        {
            ProductId = productId;
            Units = units;
        }
    }
}