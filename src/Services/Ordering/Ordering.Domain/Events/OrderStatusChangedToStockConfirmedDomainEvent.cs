namespace Ordering.Domain.Events
{
    using MediatR;

    /// <summary>
    /// Event used when the order stock items are confirmed
    /// </summary>
    public class OrderStatusChangedToStockConfirmedDomainEvent
        : INotification
    {
        public int OrderId { get; }
        public int TenantId { get; set; }

        public OrderStatusChangedToStockConfirmedDomainEvent(int orderId)
            => OrderId = orderId;

        public OrderStatusChangedToStockConfirmedDomainEvent withTenantId(int tenantId)
        {
            this.TenantId = tenantId;
            return this;
        }
    }
}