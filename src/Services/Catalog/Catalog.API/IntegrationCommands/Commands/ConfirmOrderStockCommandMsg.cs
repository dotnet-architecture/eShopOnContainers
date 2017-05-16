namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationCommands.Commands
{
    using BuildingBlocks.EventBus.Events;
    using System.Collections.Generic;

    public class ConfirmOrderStockCommandMsg : IntegrationEvent
    {
        public int OrderId { get; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; }

        public ConfirmOrderStockCommandMsg(int orderId,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
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