using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.API.Application.IntegrationCommands.Commands
{
    public class ConfirmGracePeriodCommandMsg : IntegrationEvent
    {
        public int OrderId { get; }

        public ConfirmGracePeriodCommandMsg(int orderId) =>
            OrderId = orderId;
    }
}
