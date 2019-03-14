namespace Microsoft.eShopOnContainers.Services.Marketing.API.IntegrationEvents.Events
{
    using Marketing.API.Model;
    using System.Collections.Generic;

    public class UserLocationUpdatedIntegrationEvent 
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
