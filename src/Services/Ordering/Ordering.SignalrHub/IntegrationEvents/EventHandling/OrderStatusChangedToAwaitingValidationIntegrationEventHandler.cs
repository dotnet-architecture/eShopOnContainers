using Microsoft.AspNetCore.SignalR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.SignalrHub.IntegrationEvents
{
    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
        {
            await _hubContext.Clients
                .Group(@event.BuyerName)
                .SendAsync("UpdatedOrderState", new { OrderId = @event.OrderId, Status = @event.OrderStatus });
        }
    }
}
