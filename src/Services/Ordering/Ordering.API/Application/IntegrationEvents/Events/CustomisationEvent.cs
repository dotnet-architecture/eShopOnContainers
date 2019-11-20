using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Ordering.API.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class CustomisationEvent : IntegrationEvent
    {
        public CustomisationEvent(int tenantId, IntegrationEvent @event)
        {
            TenantId = tenantId;
            eventType = @event.GetType().Name;
            //TODO
            userCheckoutAcceptedIntegrationEvent = (UserCheckoutAcceptedIntegrationEvent)@event;
        }

        public int TenantId { get; set; }
        public String eventType { get; set; }
        public UserCheckoutAcceptedIntegrationEvent userCheckoutAcceptedIntegrationEvent { get; set; }
    }
}
