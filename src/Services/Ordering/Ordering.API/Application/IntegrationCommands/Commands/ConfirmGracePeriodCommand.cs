using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.API.Application.IntegrationCommands.Commands
{
    public class ConfirmGracePeriodCommand : IntegrationEvent
    {
        public int OrderId { get; }

        public ConfirmGracePeriodCommand(int orderId) =>
            OrderId = orderId;
    }
}
