using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Webhooks.API.IntegrationEvents
{
    public class OrderStatusChangedToShippedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; private set; }
        public string OrderStatus { get; private set; }
        public string BuyerName { get; private set; }

        public OrderStatusChangedToShippedIntegrationEvent(int orderId, string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
