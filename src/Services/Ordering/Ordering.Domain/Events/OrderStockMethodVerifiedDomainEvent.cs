namespace Ordering.Domain.Events
{
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

    /// <summary>
    /// Event used when the order stock items are verified
    /// </summary>
    public class OrderStockMethodVerifiedDomainEvent
        : IAsyncNotification
    {
        public int OrderId { get; }
        public OrderStatus OrderStatus { get; }

        public OrderStockMethodVerifiedDomainEvent(int orderId,
            OrderStatus orderStatus)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
        }
    }
}