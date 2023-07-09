namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderCompletedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderCompletedIntegrationEvent(int orderId) =>
        OrderId = orderId;
}

