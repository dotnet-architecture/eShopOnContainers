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
            //TODO: must update the order state to cancelled and the CurrentOrderStateContextDescription with the reasons of no-stock confirm 
            //TODO: for this/that articles which is info coming in that integration event. --> ORDER PROCESS 

            throw new NotImplementedException();
        }
    }
}