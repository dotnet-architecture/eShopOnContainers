using System;
using Newtonsoft.Json;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            CheckForCustomisation = true;
            TenantId = 1;
        }

        public IntegrationEvent(Boolean checkForCustomisation)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            CheckForCustomisation = checkForCustomisation;
            TenantId = 1;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
            TenantId = 1;
        }

        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }

        [JsonProperty]
        public Boolean CheckForCustomisation { get; set; }
        
        //TODO fix this somehow
        [JsonProperty]
        public int TenantId { get; set; }
    }
}
