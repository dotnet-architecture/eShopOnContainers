using Ordering.API.Application.IntegrationEvents.Events;

namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderCouponRejectedIntegrationEventHandler : IIntegrationEventHandler<OrderCouponRejectedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public OrderCouponRejectedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(OrderCouponRejectedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            Log.Information("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            Log.Warning("Discount failed, cancelling order {OrderId}", @event.OrderId);

            var command = new CancelOrderCommand(@event.OrderId);

            Log.Information("----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                command.GetGenericTypeName(),
                nameof(command.OrderNumber),
                command.OrderNumber,
                command);

            await _mediator.Send(command);
        }
    }
}