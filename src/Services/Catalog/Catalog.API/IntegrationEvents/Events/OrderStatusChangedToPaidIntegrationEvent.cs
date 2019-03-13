namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events
{
    using System.Collections.Generic;

    public class OrderStatusChangedToPaidIntegrationEvent 
    {
        public int OrderId { get; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; }

        public OrderStatusChangedToPaidIntegrationEvent(int orderId,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }
}