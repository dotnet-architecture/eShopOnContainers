namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

    public class OrderStockConfirmedIntegrationEventHandler : 
        IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderStockConfirmedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(OrderStockConfirmedIntegrationEvent @event)
        {
            // Simulate a work time for confirming the stock
            await Task.Delay(10000);

            var orderToUpdate = await _orderRepository.GetAsync(@event.OrderId);

            orderToUpdate.SetStockConfirmedStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}