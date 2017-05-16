namespace Payment.API.IntegrationEvents
{ 
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using System;

    public class PaymentIntegrationEventService : IPaymentIntegrationEventService
    {
        private readonly IEventBus _eventBus;

        public PaymentIntegrationEventService(IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public void PublishThroughEventBus(IntegrationEvent evt)
        {
            _eventBus.Publish(evt); ;
        }
    }
}