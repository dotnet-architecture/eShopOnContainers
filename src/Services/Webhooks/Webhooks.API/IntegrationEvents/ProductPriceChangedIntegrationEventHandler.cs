using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System.Threading.Tasks;

namespace Webhooks.API.IntegrationEvents
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        public async Task Handle(ProductPriceChangedIntegrationEvent @event)
        {
            int i = 0;
        }
    }
}
