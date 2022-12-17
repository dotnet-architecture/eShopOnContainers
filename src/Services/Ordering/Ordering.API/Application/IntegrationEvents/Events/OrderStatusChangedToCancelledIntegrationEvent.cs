namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToCancelledIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string OrderStatus { get; }
    public string BuyerName { get; }
    public string DiscountCode { get; }

    public OrderStatusChangedToCancelledIntegrationEvent(int orderId, string orderStatus, string buyerName, string discountCode)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
        DiscountCode = discountCode;
    }
}
