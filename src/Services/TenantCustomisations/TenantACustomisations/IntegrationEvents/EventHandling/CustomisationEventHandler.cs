using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantACustomisations.Services;

namespace TenantACustomisations.IntegrationEvents.EventHandling
{
    public class CustomisationEventHandler : IIntegrationEventHandler<CustomisationEvent>
    {

        private readonly ILogger<CustomisationEventHandler> _logger;
        private readonly IEventBus _eventBus;
        private readonly IValidationService validationService;

        public CustomisationEventHandler(ILogger<CustomisationEventHandler> logger, IEventBus eventBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            validationService = new ValidationService();
        }

        public async Task Handle(CustomisationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
            IntegrationEvent integrationEvent = @event.@event;

            switch (integrationEvent.GetType().Name)
            {
                case "UserCheckoutAcceptedIntegrationEvent":
                    if (validationService.Validate((UserCheckoutAcceptedIntegrationEvent)integrationEvent))
                    {
                        integrationEvent.CheckForCustomisation = false;
                        _eventBus.Publish(integrationEvent);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
