namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events;

public record ProductDeletedIntegrationEvent : IntegrationEvent
{
    public int ProductId { get; private init; }

    public ProductDeletedIntegrationEvent(int productId)
    {
        ProductId = productId;
    }
}