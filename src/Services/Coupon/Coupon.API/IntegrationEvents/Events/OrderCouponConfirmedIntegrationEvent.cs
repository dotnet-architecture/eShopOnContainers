using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.API.IntegrationEvents.Events
{
    public record OrderCouponConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public OrderCouponConfirmedIntegrationEvent(int orderId)
            => (OrderId) = (orderId);
    }
}
