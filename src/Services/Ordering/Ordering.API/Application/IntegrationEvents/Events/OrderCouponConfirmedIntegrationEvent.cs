namespace Ordering.API.Application.IntegrationEvents.Events
{
    public record OrderCouponConfirmedIntegrationEvent: IntegrationEvent
    {
        public int OrderId { get; set; }

        public OrderCouponConfirmedIntegrationEvent(int orderId)
            => (OrderId) = (orderId);
    }
}
