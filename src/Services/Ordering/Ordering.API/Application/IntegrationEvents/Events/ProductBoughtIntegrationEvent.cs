namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record ProductBoughtIntegrationEvent : IntegrationEvent
{
    public int ProductId { get; }
    public int Units { get; }
    
    public decimal Price { get; }

    public ProductBoughtIntegrationEvent(int productId, int units, decimal price)
    {
        ProductId = productId;
        Units = units;
        Price = price;
    }
}