using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events
{
    public class CustomisationEvent : IntegrationEvent
    {
        public CustomisationEvent(int tenantId, IntegrationEvent @event)
        {
            TenantId = tenantId;
            this.@event = @event;
        }

        public int TenantId { get; set; }
        public IntegrationEvent @event { get; set; }
    }
}
