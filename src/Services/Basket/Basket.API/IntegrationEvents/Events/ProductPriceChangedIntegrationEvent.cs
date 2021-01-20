using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Services.Basket.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
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
