using Ordering.API.Application.Commands;
using Ordering.API.Application.IntegrationEvents.Events;

namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderCouponConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderCouponConfirmedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public OrderCouponConfirmedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(OrderCouponConfirmedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            Log.Information("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            var command = new CouponConfirmedCommand(@event.OrderId, @event.Discount);

            Log.Information("----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                command.GetGenericTypeName(),
                nameof(command.OrderNumber),
                command.OrderNumber,
                command);

            await _mediator.Send(command);
        }
    }
}
