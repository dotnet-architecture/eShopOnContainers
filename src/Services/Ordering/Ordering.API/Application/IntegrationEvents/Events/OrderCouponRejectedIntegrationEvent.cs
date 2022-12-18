namespace Ordering.API.Application.IntegrationEvents.Events
{
    public record OrderCouponRejectedIntegrationEvent: IntegrationEvent
    {
        public int OrderId { get; set; }

        public OrderCouponRejectedIntegrationEvent(int orderId)
            => (OrderId) = (orderId);
    }
}
