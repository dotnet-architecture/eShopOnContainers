namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions;
    using System.Threading.Tasks;
    using Events;
    using System.Linq;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using MediatR;
    using Ordering.API.Application.Commands;
    using Microsoft.Extensions.Logging;
    using Serilog.Context;
    using Microsoft.eShopOnContainers.Services.Ordering.API;
    using Ordering.API.Application.Behaviors;

    public class OrderStockRejectedIntegrationEventHandler : IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderStockRejectedIntegrationEventHandler> _logger;

        public OrderStockRejectedIntegrationEventHandler(
            IMediator mediator,
            ILogger<OrderStockRejectedIntegrationEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStockRejectedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                var orderStockRejectedItems = @event.OrderStockItems
                    .FindAll(c => !c.HasStock)
                    .Select(c => c.ProductId)
                    .ToList();

                var command = new SetStockRejectedOrderStatusCommand(@event.OrderId, orderStockRejectedItems);

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