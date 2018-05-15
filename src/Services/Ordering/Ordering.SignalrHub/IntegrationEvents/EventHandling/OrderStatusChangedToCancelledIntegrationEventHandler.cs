using Microsoft.AspNetCore.SignalR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Ordering.SignalrHub.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.SignalrHub.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToCancelledIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToCancelledIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public OrderStatusChangedToCancelledIntegrationEventHandler(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(OrderStatusChangedToCancelledIntegrationEvent @event)
        {
            await _hubContext.Clients
                .Group(@event.BuyerName)
                .SendAsync("UpdatedOrderState", new { OrderId = @event.OrderId, Status = @event.OrderStatus });
        }
    }
}