using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class OrderPaymentSuccededIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public OrderPaymentSuccededIntegrationEvent(int orderId) => OrderId = orderId;
    }
}
