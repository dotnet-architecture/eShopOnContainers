namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStockConfirmedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderStockConfirmedIntegrationEvent(int orderId) => OrderId = orderId;
}
