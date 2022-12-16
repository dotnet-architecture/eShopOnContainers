using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.API.IntegrationEvents.Events
{
    public record OrderCouponConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public int Discount { get; }

        public OrderCouponConfirmedIntegrationEvent(int orderId, int discount)
        {
            OrderId = orderId;
            Discount = discount;
        }
    }
}
