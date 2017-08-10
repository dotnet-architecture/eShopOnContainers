namespace Ordering.Domain.Events
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using System.Collections.Generic;

    /// <summary>
    /// Event used when the order is paid
    /// </summary>
    public class OrderStatusChangedToPaidDomainEvent
        : INotification
    {
        public int OrderId { get; }
        public IEnumerable<OrderItem> OrderItems { get; }

        public OrderStatusChangedToPaidDomainEvent(int orderId,
            IEnumerable<OrderItem> orderItems)
        {
            OrderId = orderId;
            OrderItems = orderItems;
        }
    }
}