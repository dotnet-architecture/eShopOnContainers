using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.API.IntegrationEvents.Events
{
    public record OrderStatusChangedToAwaitingCouponValidationIntegrationEvent: IntegrationEvent
    {
        public int OrderId { get; set; }

        public string CouponCode { get; init; }

        public OrderStatusChangedToAwaitingCouponValidationIntegrationEvent(int orderId, string couponCode)
            => (OrderId, CouponCode) = (orderId, couponCode);
    }
}
