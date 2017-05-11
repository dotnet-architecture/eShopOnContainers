using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Domain.Events
{
    using MediatR;
    using System.Collections.Generic;

    /// <summary>
    /// Event used when the order stock items are verified
    /// </summary>
    public class OrderStockConfirmedDomainEvent
        : IAsyncNotification
    {
        public int OrderId { get; }
        public OrderStatus OrderStatus { get; }

        public OrderStockConfirmedDomainEvent(int orderId,
            OrderStatus orderStatus)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
        }
    }
}