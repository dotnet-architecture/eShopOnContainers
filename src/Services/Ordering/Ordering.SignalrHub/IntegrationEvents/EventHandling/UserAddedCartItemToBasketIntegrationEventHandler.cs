using Microsoft.AspNetCore.SignalR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Ordering.SignalrHub.IntegrationEvents.Events;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.SignalrHub.IntegrationEvents.EventHandling
{
    public class UserAddedCartItemToBasketIntegrationEventHandler :
        IIntegrationEventHandler<UserAddedCartItemToBasketIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<UserAddedCartItemToBasketIntegrationEventHandler> _logger;

        public UserAddedCartItemToBasketIntegrationEventHandler(
            IHubContext<NotificationsHub> hubContext,
            ILogger<UserAddedCartItemToBasketIntegrationEventHandler> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task Handle(UserAddedCartItemToBasketIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                await _hubContext.Clients
                    .Group(@event.BuyerName)
                    .SendAsync("UpdateBasketCount", "test message");
            }
        }
    }
}