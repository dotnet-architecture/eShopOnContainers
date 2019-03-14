using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Ordering.SignalrHub.IntegrationEvents.Events;
using Serilog.Context;
using System;
using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Ordering.SignalrHub.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToShippedIntegrationEventHandler : ICapSubscribe
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<OrderStatusChangedToShippedIntegrationEventHandler> _logger;

        public OrderStatusChangedToShippedIntegrationEventHandler(
            IHubContext<NotificationsHub> hubContext,
            ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //TODO [CapSubscribe(nameof(OrderStatusChangedToShippedIntegrationEvent))]
        public async Task Handle(OrderStatusChangedToShippedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                await _hubContext.Clients
                    .Group(@event.BuyerName)
                    .SendAsync("UpdatedOrderState", new { OrderId = @event.OrderId, Status = @event.OrderStatus });
            }
        }
    }
}
