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
        }

        public IntegrationEvent(Boolean checkForCustomisation)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            CheckForCustomisation = checkForCustomisation;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
        }

        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }

        [JsonProperty]
        public Boolean CheckForCustomisation { get; set; }
    }
}
