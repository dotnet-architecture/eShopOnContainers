using System;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;
using Serilog.Context;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    public class UserCheckoutAcceptedIntegrationEventHandler : IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserCheckoutAcceptedIntegrationEventHandler> _logger;

        public UserCheckoutAcceptedIntegrationEventHandler(
            IMediator mediator,
            ILogger<UserCheckoutAcceptedIntegrationEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Integration event handler which starts the create order process
        /// </summary>
        /// <param name="eventMsg">
        /// Integration event message which is sent by the
        /// basket.api once it has successfully process the 
        /// order items.
        /// </param>
        /// <returns></returns>
        public async Task Handle(UserCheckoutAcceptedIntegrationEvent eventMsg)
        {
            using (LogContext.PushProperty("IntegrationEventId", eventMsg.Id))
            {
                _logger.LogInformation("----- UserCheckoutAcceptedIntegrationEventHandler - Handling integration event: {IntegrationEventId} ({@IntegrationEvent})", eventMsg.Id, eventMsg);

                var result = false;

                if (eventMsg.RequestId != Guid.Empty)
                {
                    var createOrderCommand = new CreateOrderCommand(eventMsg.Basket.Items, eventMsg.UserId, eventMsg.UserName, eventMsg.City, eventMsg.Street,
                        eventMsg.State, eventMsg.Country, eventMsg.ZipCode,
                        eventMsg.CardNumber, eventMsg.CardHolderName, eventMsg.CardExpiration,
                        eventMsg.CardSecurityNumber, eventMsg.CardTypeId);

                    _logger.LogInformation("----- UserCheckoutAcceptedIntegrationEventHandler - CreateOrderCommand: {@CreateOrderCommand}", createOrderCommand);

                    var requestCreateOrder = new IdentifiedCommand<CreateOrderCommand, bool>(createOrderCommand, eventMsg.RequestId);
                    result = await _mediator.Send(requestCreateOrder);

                    if (result)
                    {
                        _logger.LogInformation("----- UserCheckoutAcceptedIntegrationEventHandler - CreateOrderCommand suceeded - RequestId: {RequestId}", eventMsg.RequestId);
                    }
                    else
                    {
                        _logger.LogWarning("----- UserCheckoutAcceptedIntegrationEventHandler - CreateOrderCommand failed - RequestId: {RequestId}", eventMsg.RequestId);
                    }
                }
                else
                {
                    _logger.LogWarning("----- UserCheckoutAcceptedIntegrationEventHandler - Invalid IntegrationEvent - RequestId is missing}");
                }
            }
        }
    }
}