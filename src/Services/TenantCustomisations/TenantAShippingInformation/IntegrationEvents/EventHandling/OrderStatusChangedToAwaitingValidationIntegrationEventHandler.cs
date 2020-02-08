using Microsoft.AspNetCore.SignalR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TenantAShippingInformation.IntegrationEvents.Events;

namespace TenantAShippingInformation.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler :
        IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
    {
        private readonly ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> _logger;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}- TenantA"))
            {
                //TODO
                Debug.WriteLine(@event);

            }
        }
    }
}