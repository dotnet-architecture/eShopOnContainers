using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Webhooks.API.IntegrationEvents
{
    public record ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public int ProductId { get; private init; }

        public decimal NewPrice { get; private init; }

        public decimal OldPrice { get; private init; }

        public ProductPriceChangedIntegrationEvent(int productId, decimal newPrice, decimal oldPrice)
        {
            ProductId = productId;
            NewPrice = newPrice;
            OldPrice = oldPrice;
        }
    }
}
