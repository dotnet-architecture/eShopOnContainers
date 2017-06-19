namespace Microsoft.eShopOnContainers.Services.Locations.API.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using System.Collections.Generic;

    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; private set; }
        public List<UserLocationDetails> LocationList { get; private set; }

        public UserLocationUpdatedIntegrationEvent(string userId, List<UserLocationDetails> locationList)
        {
            UserId = userId;
            LocationList = locationList;
        }
    }
}
