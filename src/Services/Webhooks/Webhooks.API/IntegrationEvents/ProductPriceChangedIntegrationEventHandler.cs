using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Webhooks.API.IntegrationEvents
{
    public class ProductPriceChangedIntegrationEventHandler : ICapSubscribe
    {
        //TODO [CapSubscribe(nameof(ProductPriceChangedIntegrationEvent))]
        public Task Handle(ProductPriceChangedIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
