using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.Behaviors;
using Serilog.Context;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using TenantACustomisations.IntegrationEvents.Events;
using TenantACustomisations.ExternalServices;
using TenantACustomisations.Database;

namespace TenantACustomisations.IntegrationEvents.EventHandling
{
    public class TenantAUserCheckoutAcceptedIntegrationEventHandler : 
        IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>
    {
        private readonly IMediator _mediator;
        private readonly IEventBus _eventBus;
        private readonly ILogger<TenantAUserCheckoutAcceptedIntegrationEventHandler> _logger;
        //private readonly TenantAContext _context;
        //private readonly IShippingService _shippingService;

        public TenantAUserCheckoutAcceptedIntegrationEventHandler(
            IMediator mediator,
            ILogger<TenantAUserCheckoutAcceptedIntegrationEventHandler> logger, 
            IEventBus eventBus
            )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// Integration event handler which starts the create order process
        /// </summary>
        /// <param name="@event">
        /// Integration event message which is sent by the
        /// basket.api once it has successfully process the 
        /// order items.
        /// </param>
        /// <returns></returns>
        public async Task Handle(UserCheckoutAcceptedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}- TenantA"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at TenantA- ({@IntegrationEvent})", @event.Id, @event);
                _logger.LogInformation("Hello");
                
                //TODO
                Debug.WriteLine(@event);
                //Save shipping info
                //Hard code view comp
                //Retrieve shipping info and show
            }
        }
    }
}