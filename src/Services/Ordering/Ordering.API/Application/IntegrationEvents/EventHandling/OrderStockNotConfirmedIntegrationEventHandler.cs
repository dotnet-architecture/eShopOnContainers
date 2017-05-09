namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System;
    using System.Threading.Tasks;
    using Events;

    public class OrderStockNotConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStockNotConfirmedIntegrationEvent>
    {
        public async Task Handle(OrderStockNotConfirmedIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}