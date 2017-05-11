using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public OrderPaymentFailedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}
