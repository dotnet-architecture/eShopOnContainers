namespace Payment.API.IntegrationCommands.CommandHandlers
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Payment.API.IntegrationCommands.Commands;
    using System.Threading.Tasks;
    using System;
    using Payment.API.IntegrationEvents;
    using Payment.API.IntegrationEvents.Events;

    public class PayOrderCommandMsgHandler : IIntegrationEventHandler<PayOrderCommandMsg>
    {
        private readonly IPaymentIntegrationEventService _paymentIntegrationEventService;

        public PayOrderCommandMsgHandler(IPaymentIntegrationEventService paymentIntegrationEventService)
        {
            _paymentIntegrationEventService = paymentIntegrationEventService;
        }


        public async Task Handle(PayOrderCommandMsg @event)
        {
            //PAYMENT SUCCESSED
            var orderPaymentSuccededIntegrationEvent = new OrderPaymentSuccededIntegrationEvent(@event.OrderId);
            _paymentIntegrationEventService.PublishThroughEventBus(orderPaymentSuccededIntegrationEvent);

            //PAYMENT FAILED
            //var orderPaymentFailedIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
            //_paymentIntegrationEventService.PublishThroughEventBus(orderPaymentFailedIntegrationEvent);
        }
    }
}
