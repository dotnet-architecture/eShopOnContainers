namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using MediatR;
    using System;
    using Ordering.API.Application.Commands;
    using Microsoft.Extensions.Logging;
    using Serilog.Context;
    using Microsoft.eShopOnContainers.Services.Ordering.API;

    public class OrderStockConfirmedIntegrationEventHandler :
        IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>
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

        public async Task Handle(OrderStockConfirmedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventId_Context", @event.Id))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppShortName} - ({@IntegrationEvent})", @event.Id, Program.AppShortName, @event);

                var command = new SetStockConfirmedOrderStatusCommand(@event.OrderId);
                await _mediator.Send(command);
            }
        }
    }
}