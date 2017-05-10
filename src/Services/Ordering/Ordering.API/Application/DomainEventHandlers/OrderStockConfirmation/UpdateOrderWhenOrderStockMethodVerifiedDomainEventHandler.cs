namespace Ordering.API.Application.DomainEventHandlers.OrderStartedEvent
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using Microsoft.Extensions.Logging;
    using Domain.Events;
    using System;
    using System.Threading.Tasks;

    public class UpdateOrderWhenOrderStockMethodVerifiedDomainEventHandler
                   : IAsyncNotificationHandler<OrderStockMethodVerifiedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;        

        public UpdateOrderWhenOrderStockMethodVerifiedDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Domain Logic comment:
        // When the Order Stock items method have been validate and confirmed, 
        // then we can update the original Order with the new order status
        public async Task Handle(OrderStockMethodVerifiedDomainEvent orderStockMethodVerifiedDomainEvent)
        {
            var orderToUpdate = await _orderRepository.GetAsync(orderStockMethodVerifiedDomainEvent.OrderId);
            orderToUpdate.SetOrderStatusId(orderStockMethodVerifiedDomainEvent.OrderStatus.Id);
                             
            _orderRepository.Update(orderToUpdate);

            await _orderRepository.UnitOfWork
                .SaveEntitiesAsync();

            _logger.CreateLogger(nameof(UpdateOrderWhenOrderStockMethodVerifiedDomainEventHandler))
                .LogTrace($"Order with Id: {orderStockMethodVerifiedDomainEvent.OrderId} has been successfully updated with " +
                          $"a status order id: { orderStockMethodVerifiedDomainEvent.OrderStatus.Id }");


            //var payOrderCommandMsg = new PayOrderCommandMsg(order.Id);

            //// Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            //await _orderingIntegrationEventService.SaveEventAndOrderingContextChangesAsync(payOrderCommandMsg);

            //// Publish through the Event Bus and mark the saved event as published
            //await _orderingIntegrationEventService.PublishThroughEventBusAsync(payOrderCommandMsg);
        }
    }  
}