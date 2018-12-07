namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using MediatR;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Ordering.API.Application.Commands;
    using Ordering.API.Application.IntegrationEvents.Events;
    using System;
    using System.Threading.Tasks;

    public class OrderPaymentFailedIntegrationEventHandler : 
        IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly IMediator _mediator;

        public OrderPaymentFailedIntegrationEventHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            var command = new CancelOrderCommand(@event.OrderId);
            await _mediator.Send(command);
        }
    }
}
