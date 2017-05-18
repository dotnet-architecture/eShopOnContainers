namespace Ordering.API.Application.IntegrationCommands.Commands
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class PayOrderCommand : IntegrationEvent
    {
        public int OrderId { get; }

        public PayOrderCommand(int orderId)
        {
            OrderId = orderId;
        }
    }
}