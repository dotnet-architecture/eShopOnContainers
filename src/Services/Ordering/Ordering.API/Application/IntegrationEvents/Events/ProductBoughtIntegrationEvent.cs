namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record ProductBoughtIntegrationEvent : IntegrationEvent
{
    public int ProductId { get; }
    public int Units { get; }

    public ProductBoughtIntegrationEvent(int productId, int units)
    {
        ProductId = productId;
        Units = units;
    }
}