namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.EventHandling;
   
public class OrderCompletedSucceededIntegrationEventHandler :
    IIntegrationEventHandler<OrderCompletedSucceededIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCompletedSucceededIntegrationEventHandler> _logger;

    public OrderCompletedSucceededIntegrationEventHandler(
        IMediator mediator,
        ILogger<OrderCompletedSucceededIntegrationEventHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OrderCompletedSucceededIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            var command = new SetCompletedOrderStatusCommand(@event.OrderId);

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
