using DotNetCore.CAP;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.API;
    using Microsoft.Extensions.Logging;
    using Ordering.API.Application.Commands;
    using Ordering.API.Application.IntegrationEvents.Events;
    using Serilog.Context;
    using System.Threading.Tasks;
    using System;

    public class OrderPaymentFailedIntegrationEventHandler : ICapSubscribe
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

        public OrderPaymentFailedIntegrationEventHandler(
            IMediator mediator,
            ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //TODO: [CapSubscribe(nameof(OrderPaymentFailedIntegrationEvent))]
        public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {AppName} - ({@IntegrationEvent})",  Program.AppName, @event);

                var command = new CancelOrderCommand(@event.OrderId);

                await _mediator.Send(command);
            }
        }
    }
}
