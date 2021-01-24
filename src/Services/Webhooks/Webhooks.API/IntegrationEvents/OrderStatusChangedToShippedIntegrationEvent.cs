using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Webhooks.API.IntegrationEvents
{
    public record OrderStatusChangedToShippedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; private init; }
        public string OrderStatus { get; private init; }
        public string BuyerName { get; private init; }

        public OrderStatusChangedToShippedIntegrationEvent(int orderId, string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
