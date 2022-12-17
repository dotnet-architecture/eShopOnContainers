namespace Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingCouponValidationIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public string OrderStatus { get; }

    public string BuyerName { get; }

    public string Code { get; set; }

    public OrderStatusChangedToAwaitingCouponValidationIntegrationEvent(int orderId, string orderStatus, string buyerName, string code)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
        Code = code;
    }
}