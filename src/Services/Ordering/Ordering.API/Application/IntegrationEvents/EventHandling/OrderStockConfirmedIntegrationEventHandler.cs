namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System;
    using System.Threading.Tasks;
    using Events;

    public class OrderStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>
    {
        public async Task Handle(OrderStockConfirmedIntegrationEvent @event)
        {
            //TODO: 1) Updates the state to "StockValidated" and any meaningful OrderContextDescription message saying that all the items were confirmed with available stock, etc
            //TODO: 2) Sends a Command-message (PayOrderCommand msg/bus) to the Payment svc. from Ordering micro (thru Command Bus, as a message, NOT http)

            throw new NotImplementedException();
        }
    }
}