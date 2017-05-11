namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Ordering.API.Application.IntegrationCommands.Commands;
    using Ordering.Domain.Exceptions;

    public class OrderStockConfirmedIntegrationEventHandler : 
        IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>
    {
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly IOrderRepository _orderRepository;

        public OrderStockConfirmedIntegrationEventHandler(IOrderRepository orderRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository;
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStockConfirmedIntegrationEvent @event)
        {
            var order = await _orderRepository.GetAsync(@event.OrderId);
            CheckValidSagaId(order);

            order.SetOrderStockConfirmed();
        }

        private void CheckValidSagaId(Order orderSaga)
        {
            if (orderSaga is null)
            {
                throw new OrderingDomainException("Not able to process order saga event. Reason: no valid orderId");
            }
        }
    }
}