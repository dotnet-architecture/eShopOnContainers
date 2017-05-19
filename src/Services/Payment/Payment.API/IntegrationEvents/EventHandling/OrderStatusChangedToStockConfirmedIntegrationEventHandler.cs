namespace Payment.API.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Payment.API.IntegrationEvents.Events;
    using Microsoft.Extensions.Options;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class OrderStatusChangedToStockConfirmedIntegrationEventHandler : 
        IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly PaymentSettings _settings;

        public OrderStatusChangedToStockConfirmedIntegrationEventHandler(IEventBus eventBus, 
            IOptionsSnapshot<PaymentSettings> settings)
        {
            _eventBus = eventBus;
            _settings = settings.Value;
        }         

        public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
        {
            IntegrationEvent orderPaymentIntegrationEvent;
            if(_settings.PaymentSucceded)
            {
                orderPaymentIntegrationEvent = new OrderPaymentSuccededIntegrationEvent(@event.OrderId);
            }
            else
            {
                orderPaymentIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
            }

            _eventBus.Publish(orderPaymentIntegrationEvent);
        }
    }
}