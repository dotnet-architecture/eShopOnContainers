using System;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;
using Serilog.Context;
using Microsoft.eShopOnContainers.Services.Ordering.API;

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
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppShortName} - ({@IntegrationEvent})", eventMsg.Id, Program.AppShortName, eventMsg);

                var result = false;

                if (eventMsg.RequestId != Guid.Empty)
                {
                    using (LogContext.PushProperty("IdentifiedCommandId", eventMsg.RequestId))
                    {
                        var createOrderCommand = new CreateOrderCommand(eventMsg.Basket.Items, eventMsg.UserId, eventMsg.UserName, eventMsg.City, eventMsg.Street,
                            eventMsg.State, eventMsg.Country, eventMsg.ZipCode,
                            eventMsg.CardNumber, eventMsg.CardHolderName, eventMsg.CardExpiration,
                            eventMsg.CardSecurityNumber, eventMsg.CardTypeId);

                        var requestCreateOrder = new IdentifiedCommand<CreateOrderCommand, bool>(createOrderCommand, eventMsg.RequestId);

                        _logger.LogInformation("----- IdentifiedCreateOrderCommand: {@IdentifiedCreateOrderCommand}", requestCreateOrder);

                        result = await _mediator.Send(requestCreateOrder);

                        if (result)
                        {
                            _logger.LogInformation("----- CreateOrderCommand suceeded - RequestId: {RequestId}", eventMsg.RequestId);
                        }
                        else
                        {
                            _logger.LogWarning("----- CreateOrderCommand failed - RequestId: {RequestId}", eventMsg.RequestId);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("----- Invalid IntegrationEvent - RequestId is missing - {@IntegrationEvent}", eventMsg);
                }
            }
        }
    }
}