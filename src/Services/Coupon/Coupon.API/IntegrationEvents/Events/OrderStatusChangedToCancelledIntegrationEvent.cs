using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Newtonsoft.Json;

namespace Coupon.API.IntegrationEvents.Events
{
    public record OrderStatusChangedToCancelledIntegrationEvent : IntegrationEvent
    {
        [JsonProperty]
        public int OrderId { get; private set; }

        [JsonProperty]
        public string OrderStatus { get; private set; }
        
        [JsonProperty]
        public string BuyerName { get; private set; }
        
        [JsonProperty]
        public string DiscountCode { get; private set; }
    }
}
