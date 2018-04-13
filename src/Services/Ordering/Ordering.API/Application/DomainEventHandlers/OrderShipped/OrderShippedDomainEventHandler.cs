using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.Extensions.Logging;
using Ordering.API.Infrastructure.Hubs;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers.OrderShipped
{
    public class OrderShippedDomainEventHandler
                   : INotificationHandler<OrderShippedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly ILoggerFactory _logger;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public OrderShippedDomainEventHandler(
            IOrderRepository orderRepository, 
            ILoggerFactory logger,
            IBuyerRepository buyerRepository,
            IHubContext<NotificationsHub> hubContext)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));            
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task Handle(OrderShippedDomainEvent orderShippedDomainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger(nameof(OrderShippedDomainEvent))
             .LogTrace($"Order with Id: {orderShippedDomainEvent.Order.Id} has been successfully updated with " +
                       $"a status order id: {OrderStatus.Shipped.Id}");

            var order = await _orderRepository.GetAsync(orderShippedDomainEvent.Order.Id);
            var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());           

            await _hubContext.Clients
                .Group(buyer.Name)
                .SendAsync("UpdatedOrderState", new { OrderId = order.Id, Status = order.OrderStatus.Name });
        }
    }
}
