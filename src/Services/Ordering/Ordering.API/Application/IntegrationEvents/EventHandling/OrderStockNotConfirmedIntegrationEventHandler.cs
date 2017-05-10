namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Ordering.API.Application.Sagas;
    using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
    using Ordering.Domain.Exceptions;

    public class OrderStockNotConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStockNotConfirmedIntegrationEvent>
    {
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly IOrderRepository _orderRepository;

        public OrderStockNotConfirmedIntegrationEventHandler(IOrderRepository orderRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository;
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStockNotConfirmedIntegrationEvent @event)
        {
            //TODO: must update the order state to cancelled and the CurrentOrderStateContextDescription with the reasons of no-stock confirm 
            var order = await _orderRepository.GetAsync(@event.OrderId);
            CheckValidSagaId(order);

            order.SetOrderStockConfirmed(false);

            var orderStockNotConfirmedItems = @event.OrderStockItems.FindAll(c => !c.Confirmed); 

            foreach (var orderStockNotConfirmedItem in orderStockNotConfirmedItems)
            {
                //TODO: Add messages
            }
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