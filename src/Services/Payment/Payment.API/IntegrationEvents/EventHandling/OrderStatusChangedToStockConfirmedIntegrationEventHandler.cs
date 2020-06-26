namespace Payment.API.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Payment.API.IntegrationEvents.Events;
    using Serilog.Context;
    using System.Threading.Tasks;

    public class OrderStatusChangedToStockConfirmedIntegrationEventHandler :
        IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly PaymentSettings _settings;
        private readonly ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> _logger;

        public OrderStatusChangedToStockConfirmedIntegrationEventHandler(
            IEventBus eventBus,
            IOptionsSnapshot<PaymentSettings> settings,
            ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger)
        {
            _eventBus = eventBus;
            _settings = settings.Value;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));

            _logger.LogTrace("PaymentSettings: {@PaymentSettings}", _settings);
        }

        public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                IntegrationEvent orderPaymentIntegrationEvent;

                //Business feature comment:
                // When OrderStatusChangedToStockConfirmed Integration Event is handled.
                // Here we're simulating that we'd be performing the payment against any payment gateway
                // Instead of a real payment we just take the env. var to simulate the payment 
                // The payment can be successful or it can fail

                if (_settings.PaymentSucceeded)
                {
                    orderPaymentIntegrationEvent = new OrderPaymentSucceededIntegrationEvent(@event.OrderId);
                }
                else
                {
                    orderPaymentIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
                }

                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", orderPaymentIntegrationEvent.Id, Program.AppName, orderPaymentIntegrationEvent);

                _eventBus.Publish(orderPaymentIntegrationEvent);

                await Task.CompletedTask;
            }
        }
    }
}