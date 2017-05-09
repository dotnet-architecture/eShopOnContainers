namespace Ordering.API.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderStockNotConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        //public IEnumerable<Item> { get; }

        public OrderStockNotConfirmedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}