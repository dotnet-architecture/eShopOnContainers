using DotNetCore.CAP;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using System.Threading.Tasks;
    using Events;
    using MediatR;
    using System;
    using Ordering.API.Application.Commands;
    using Microsoft.Extensions.Logging;
    using Serilog.Context;
    using Microsoft.eShopOnContainers.Services.Ordering.API;

    public class OrderStockConfirmedIntegrationEventHandler :ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderStockConfirmedIntegrationEventHandler> _logger;

        public OrderStockConfirmedIntegrationEventHandler(
            IMediator mediator,
            ILogger<OrderStockConfirmedIntegrationEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [CapSubscribe(nameof(OrderStockConfirmedIntegrationEvent))]
        public async Task Handle(OrderStockConfirmedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {AppName} - ({@IntegrationEvent})", Program.AppName, @event);

                var command = new SetStockConfirmedOrderStatusCommand(@event.OrderId);

                await _mediator.Send(command);
            }
        }
    }
}