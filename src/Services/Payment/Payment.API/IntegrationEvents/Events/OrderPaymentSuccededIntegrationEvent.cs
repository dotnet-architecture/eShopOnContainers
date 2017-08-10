namespace Payment.API.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderPaymentSuccededIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public OrderPaymentSuccededIntegrationEvent(int orderId) => OrderId = orderId;
    }
}