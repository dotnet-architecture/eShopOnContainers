namespace Payment.API.IntegrationCommands.CommandHandlers
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Payment.API.IntegrationCommands.Commands;
    using System.Threading.Tasks;
    using Payment.API.IntegrationEvents;
    using Payment.API.IntegrationEvents.Events;

    public class PayOrderCommandHandler : IIntegrationEventHandler<PayOrderCommand>
    {
        private readonly IPaymentIntegrationEventService _paymentIntegrationEventService;

        public PayOrderCommandHandler(IPaymentIntegrationEventService paymentIntegrationEventService)
            => _paymentIntegrationEventService = paymentIntegrationEventService;

        public async Task Handle(PayOrderCommand @event)
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