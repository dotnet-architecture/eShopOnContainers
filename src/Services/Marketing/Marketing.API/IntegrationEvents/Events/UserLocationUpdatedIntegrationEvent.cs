namespace Microsoft.eShopOnContainers.Services.Marketing.API.IntegrationEvents.Events
{
    using Model;
    using System.Collections.Generic;
    using BuildingBlocks.EventBus.Events;

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
