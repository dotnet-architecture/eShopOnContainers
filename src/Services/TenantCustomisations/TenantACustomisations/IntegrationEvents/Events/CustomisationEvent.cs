using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Ordering.API.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TenantACustomisations.IntegrationEvents.Events
{
    public class CustomisationEvent : IntegrationEvent
    {
        public CustomisationEvent(int tenantId, IntegrationEvent @event)
        {
            TenantId = tenantId;
            this.@event = @event;
            eventType = @event.GetType().Name;
            //TODO
            userCheckoutAcceptedIntegrationEvent = (UserCheckoutAcceptedIntegrationEvent)@event;
        }

        public int TenantId { get; set; }
        public IntegrationEvent @event { get; set; }
        public String eventType { get; set; }
        public UserCheckoutAcceptedIntegrationEvent userCheckoutAcceptedIntegrationEvent { get; set; }
    }
}
