namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Events;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using System.Linq;
    using System.Threading.Tasks;

    public class OrderStockRejectedIntegrationEventHandler
        : IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderStockRejectedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(OrderStockRejectedIntegrationEvent @event)
        {
            var orderToUpdate = await _orderRepository.GetAsync(@event.OrderId);

            var orderStockRejectedItems = @event.OrderStockItems
                .FindAll(c => !c.HasStock)
                .Select(c => c.ProductId);

            orderToUpdate.SetCancelledStatusWhenStockIsRejected(orderStockRejectedItems);

            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}