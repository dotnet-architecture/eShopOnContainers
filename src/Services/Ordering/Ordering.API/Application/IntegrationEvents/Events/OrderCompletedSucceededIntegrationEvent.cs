namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderCompletedSucceededIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderCompletedSucceededIntegrationEvent(int orderId) => OrderId = orderId;
}
