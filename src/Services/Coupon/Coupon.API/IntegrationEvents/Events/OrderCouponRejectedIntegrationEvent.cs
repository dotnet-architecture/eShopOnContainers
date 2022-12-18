using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.API.IntegrationEvents.Events
{
    public class OrderCouponRejectedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public string Code { get; }

        public OrderCouponRejectedIntegrationEvent(int orderId, string code)
        {
            OrderId = orderId;
            Code = code;
        }
    }
}
