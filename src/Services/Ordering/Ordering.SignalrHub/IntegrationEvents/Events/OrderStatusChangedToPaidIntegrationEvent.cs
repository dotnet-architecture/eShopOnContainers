using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.SignalrHub.IntegrationEvents.Events
{
    public record OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }

        public OrderStatusChangedToPaidIntegrationEvent(int orderId,
            string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
