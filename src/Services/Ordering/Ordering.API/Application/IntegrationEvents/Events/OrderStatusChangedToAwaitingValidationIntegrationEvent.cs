namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;
  
public record OrderStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string OrderStatus { get; }
    public string BuyerName { get; }
    public IEnumerable<OrderStockItem> OrderStockItems { get; }

    public OrderStatusChangedToAwaitingValidationIntegrationEvent(int orderId, string orderStatus, string buyerName,
        IEnumerable<OrderStockItem> orderStockItems)
    {
        OrderId = orderId;
        OrderStockItems = orderStockItems;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
    }
}

public record OrderStockItem
{
    public int ProductId { get; }
    public int Units { get; }

    public decimal Price { get; }

    public OrderStockItem(int productId, int units, decimal price = Decimal.Zero)
    {
        ProductId = productId;
        Units = units;
        Price = price;
    }
}
