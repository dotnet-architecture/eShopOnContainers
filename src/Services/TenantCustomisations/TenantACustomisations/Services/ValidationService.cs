using Ordering.API.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
