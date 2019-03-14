using DotNetCore.CAP;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.API;
    using Microsoft.Extensions.Logging;
    using Ordering.API.Application.Commands;
    using Ordering.API.Application.IntegrationEvents.Events;
    using Serilog.Context;
    using System;
    using System.Threading.Tasks;

    public class OrderPaymentSuccededIntegrationEventHandler : ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderPaymentSuccededIntegrationEventHandler> _logger;

        public OrderPaymentSuccededIntegrationEventHandler(
            IMediator mediator,
            ILogger<OrderPaymentSuccededIntegrationEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [CapSubscribe(nameof(OrderPaymentSuccededIntegrationEvent))]
        public async Task Handle(OrderPaymentSuccededIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {AppName} - ({@IntegrationEvent})", Program.AppName, @event);

                var command = new SetPaidOrderStatusCommand(@event.OrderId);

                await _mediator.Send(command);
            }
        }
    }
}