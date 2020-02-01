namespace Ordering.Domain.Events
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using System.Collections.Generic;

    /// <summary>
    /// Event used when the grace period order is confirmed
    /// </summary>
    public class OrderStatusChangedToAwaitingValidationDomainEvent
         : INotification
    {
        public int OrderId { get; }
        public IEnumerable<OrderItem> OrderItems { get; }
        public int TenantId { get; }

        public OrderStatusChangedToAwaitingValidationDomainEvent(int orderId,
            IEnumerable<OrderItem> orderItems,
            int tenantId)
        {
            OrderId = orderId;
            OrderItems = orderItems;
            TenantId = tenantId;
        }
    }
}