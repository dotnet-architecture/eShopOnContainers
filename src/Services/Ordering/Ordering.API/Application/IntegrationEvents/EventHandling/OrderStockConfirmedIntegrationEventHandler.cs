namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Ordering.API.Application.IntegrationCommands.Commands;
    using Ordering.Domain.Exceptions;

    public class OrderStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>
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
            //TODO: 1) Updates the state to "StockValidated" and any meaningful OrderContextDescription message saying that all the items were confirmed with available stock, etc
            var order = await _orderRepository.GetAsync(@event.OrderId);
            CheckValidSagaId(order);

            order.SetOrderStockConfirmed(true);

            //Create Integration Event to be published through the Event Bus
            var payOrderCommandMsg = new PayOrderCommandMsg(order.Id);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _orderingIntegrationEventService.SaveEventAndOrderingContextChangesAsync(payOrderCommandMsg);

            // Publish through the Event Bus and mark the saved event as published
            await _orderingIntegrationEventService.PublishThroughEventBusAsync(payOrderCommandMsg);
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