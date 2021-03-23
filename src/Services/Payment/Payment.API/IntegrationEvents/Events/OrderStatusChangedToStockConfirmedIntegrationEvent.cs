namespace Payment.API.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public record OrderStatusChangedToStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public OrderStatusChangedToStockConfirmedIntegrationEvent(int orderId)
            => OrderId = orderId;
    }
}