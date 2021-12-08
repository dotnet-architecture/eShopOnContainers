namespace Ordering.API.Application.IntegrationEvents.EventHandling;

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
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            var command = new SetStockConfirmedOrderStatusCommand(@event.OrderId);

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
