namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Ordering.API.Application.IntegrationEvents.Events;
    using System.Threading.Tasks;

    public class OrderPaymentFailedIntegrationEventHandler : 
        IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
        }
    }
}
