namespace Ordering.API.Application.IntegrationCommands.Commands
{
    using System.Collections.Generic;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ConfirmOrderStockCommandMsg : IntegrationEvent
    {
        public int OrderId { get; }
        public IEnumerable<OrderStockItem> OrderStockItem { get; }

        public ConfirmOrderStockCommandMsg(int orderId,
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