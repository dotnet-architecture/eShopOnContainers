namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Ordering.API.Application.IntegrationEvents.Events;
    using System.Threading.Tasks;

    public class OrderPaymentSuccededIntegrationEventHandler : 
        IIntegrationEventHandler<OrderPaymentSuccededIntegrationEvent>
    {
        public async Task Handle(OrderPaymentSuccededIntegrationEvent @event)
        {
        }
    }
}
