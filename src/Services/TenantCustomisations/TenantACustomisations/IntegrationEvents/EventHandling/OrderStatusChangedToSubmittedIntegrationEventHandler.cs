using Microsoft.AspNetCore.SignalR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TenantACustomisations.Database;
using TenantACustomisations.ExternalServices;
using TenantACustomisations.IntegrationEvents.Events;

namespace TenantACustomisations.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToSubmittedIntegrationEventHandler :
        IIntegrationEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>
    {
        private readonly ILogger<OrderStatusChangedToSubmittedIntegrationEventHandler> _logger;
        private readonly IShippingService _shippingService;
        private readonly TenantAContext _context;

        public OrderStatusChangedToSubmittedIntegrationEventHandler(ILogger<OrderStatusChangedToSubmittedIntegrationEventHandler> logger, IShippingService shippingService, TenantAContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _shippingService = shippingService ?? throw new ArgumentNullException(nameof(shippingService));
            _context = context ?? throw new ArgumentNullException(nameof(shippingService));
        }

        public async Task Handle(OrderStatusChangedToSubmittedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}- TenantA"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at TenantA - ({@IntegrationEvent})", @event.Id, @event);
                _logger.LogInformation("Hello");
                //TODO
                Debug.WriteLine(@event);
                ShippingInformation shippingInformation = _shippingService.CalculateShippingInformation(@event.OrderId);
                _context.ShippingInformation.Add(shippingInformation);
                _logger.LogInformation("----- Saving shipping information: {IntegrationEventId} at TenantA - ({@IntegrationEvent}) - {@ShippingInformation}", @event.Id, @event, shippingInformation);
                _context.SaveChanges();
            }
        }
    }
}