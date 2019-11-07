namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using MediatR;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions;
    using Microsoft.eShopOnContainers.Services.Ordering.API;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Microsoft.Extensions.Logging;
    using Ordering.API.Application.Behaviors;
    using Ordering.API.Application.Commands;
    using Ordering.API.Application.IntegrationEvents.Events;
    using Serilog.Context;
    using System;
    using System.Threading.Tasks;

    public class OrderPaymentSucceededIntegrationEventHandler : 
        IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderPaymentSucceededIntegrationEventHandler> _logger;

        public OrderPaymentSucceededIntegrationEventHandler(
            IMediator mediator,
            ILogger<OrderPaymentSucceededIntegrationEventHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderPaymentSucceededIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                var command = new SetPaidOrderStatusCommand(@event.OrderId);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    command.GetGenericTypeName(),
                    nameof(command.OrderNumber),
                    command.OrderNumber,
                    command);

                await _mediator.Send(command);
            }
        }
    }
}