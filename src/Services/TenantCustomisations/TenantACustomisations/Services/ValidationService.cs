using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Ordering.API.Application.IntegrationEvents.Events;

namespace TenantACustomisations.Services
{
    public class ValidationService : IValidationService
    {
        public bool Validate(UserCheckoutAcceptedIntegrationEvent @event)
        {
            return @event.State == "Oslo";
        }
    }
}
