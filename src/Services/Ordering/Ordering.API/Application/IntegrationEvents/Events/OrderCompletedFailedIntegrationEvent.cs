namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderCompletedFailedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderCompletedFailedIntegrationEvent(int orderId) => OrderId = orderId;
}
