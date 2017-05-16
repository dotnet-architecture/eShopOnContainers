namespace Ordering.API.Application.IntegrationCommands.Commands
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ShipOrderCommandMsg : IntegrationEvent
    {
        public int OrderId { get; }

        public ShipOrderCommandMsg(int orderId)
        {
            OrderId = orderId;
        }
    }
}