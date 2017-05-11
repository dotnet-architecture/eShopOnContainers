using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    public class UserCheckoutAcceptedIntegrationEventHandler : IDynamicIntegrationEventHandler
    {
        public async Task Handle(dynamic eventData)
        {
            int i = 0;
        }
    }
}
