using DotNetCore.CAP;

namespace Payment.API.IntegrationEvents.EventHandling
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Payment.API.IntegrationEvents.Events;
    using Serilog.Context;
    using System.Threading.Tasks;

    public class OrderStatusChangedToStockConfirmedIntegrationEventHandler : ICapSubscribe
    {
        private readonly ICapPublisher _eventBus;
        private readonly PaymentSettings _settings;
        private readonly ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> _logger;

        public OrderStatusChangedToStockConfirmedIntegrationEventHandler(
            ICapPublisher eventBus,
            IOptionsSnapshot<PaymentSettings> settings,
            ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger)
        {
            _eventBus = eventBus;
            _settings = settings.Value;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        //TODO: [CapSubscribe(nameof(OrderStatusChangedToStockConfirmedIntegrationEvent))]
        public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {AppName} - ({@IntegrationEvent})", Program.AppName, @event);

                //Business feature comment:
                // When OrderStatusChangedToStockConfirmed Integration Event is handled.
                // Here we're simulating that we'd be performing the payment against any payment gateway
                // Instead of a real payment we just take the env. var to simulate the payment 
                // The payment can be successful or it can fail

                if (_settings.PaymentSucceded)
                {
                    var orderPaymentSucceededIntegrationEvent = new OrderPaymentSuccededIntegrationEvent(@event.OrderId);
                    await _eventBus.PublishAsync(nameof(OrderPaymentSuccededIntegrationEvent), orderPaymentSucceededIntegrationEvent);
                    _logger.LogInformation("----- Publishing integration event: {AppName} - ({@IntegrationEvent})", Program.AppName, orderPaymentSucceededIntegrationEvent);
                }
                else
                {
                    var orderPaymentFailedIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
                    await _eventBus.PublishAsync(nameof(OrderPaymentFailedIntegrationEvent), orderPaymentFailedIntegrationEvent);
                    _logger.LogInformation("----- Publishing integration event: {AppName} - ({@IntegrationEvent})", Program.AppName, orderPaymentFailedIntegrationEvent);
                }
            }
        }
    }
}