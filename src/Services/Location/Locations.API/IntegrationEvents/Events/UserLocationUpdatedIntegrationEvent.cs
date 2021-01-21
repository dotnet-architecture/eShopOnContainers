namespace Microsoft.eShopOnContainers.Services.Locations.API.IntegrationEvents.Events
{
    using Locations.API.Model;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using System.Collections.Generic;

    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; set; }
        public List<UserLocationDetails> LocationList { get; set; }

        public UserLocationUpdatedIntegrationEvent(string userId, List<UserLocationDetails> locationList)
        {
            UserId = userId;
            LocationList = locationList;
        }
    }
}
