using Ordering.SignalrHub.IntegrationEvents.Events;

namespace Ordering.SignalrHub.IntegrationEvents.EventHandling;

public class OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent>
{
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly ILogger<OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler> _logger;

    public OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler(
        IHubContext<NotificationsHub> hubContext,
        ILogger<OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler> logger)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public async Task Handle(OrderStatusChangedToAwaitingCouponValidationIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            await _hubContext.Clients
                .Group(@event.BuyerName)
                .SendAsync("UpdatedOrderState", new { OrderId = @event.OrderId, Status = @event.OrderStatus });
        }
    }
}