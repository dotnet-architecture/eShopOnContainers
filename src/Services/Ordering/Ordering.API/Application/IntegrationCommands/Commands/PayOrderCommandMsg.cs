namespace Ordering.API.Application.IntegrationCommands.Commands
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class PayOrderCommandMsg : IntegrationEvent
    {
        public int OrderId { get; }

        public PayOrderCommandMsg(int orderId)
        {
            OrderId = orderId;
        }
    }
}