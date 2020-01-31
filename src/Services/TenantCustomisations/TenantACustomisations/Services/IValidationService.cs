using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Ordering.API.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantACustomisations.Services
{
    interface IValidationService
    {
        Boolean Validate(UserCheckoutAcceptedIntegrationEvent @event);
    }
}
