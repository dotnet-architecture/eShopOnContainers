using Newtonsoft.Json;

namespace Ordering.API.Application.IntegrationEvents.Events;

public record OrderCouponRejectedIntegrationEvent : IntegrationEvent
{
    [JsonProperty]
    public int OrderId { get; private set; }

    [JsonProperty]
    public string Code { get; private set; }
}