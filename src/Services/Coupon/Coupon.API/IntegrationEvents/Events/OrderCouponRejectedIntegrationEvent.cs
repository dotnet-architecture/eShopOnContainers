using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.API.IntegrationEvents.Events
{
    public record OrderCouponRejectedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }

        public OrderCouponRejectedIntegrationEvent(int orderId)
            => (OrderId) = (orderId);
    }
}
