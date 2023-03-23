namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events;

// Integration Events notes: 
// An Event is “something that has happened in the past”, therefore its name has to be past tense
// An Integration Event is an event that can cause side effects to other microservices, Bounded-Contexts or external systems.
public record ProductStockChangedIntegrationEvent : IntegrationEvent
{
    public int ProductId { get; private init; }

    public decimal NewStock { get; private init; }

    public decimal OldStock { get; private init; }

    public ProductStockChangedIntegrationEvent(int productId, decimal newStock, decimal oldStock)
    {
        ProductId = productId;
        NewStock = newStock;
        OldStock = oldStock;
    }
}