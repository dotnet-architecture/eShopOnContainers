namespace Microsoft.eShopOnContainers.Services.Ordering.Domain
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using System;
    using System.Collections.Generic;

    public class Order
        : Entity, IAggregateRoot
    {
        public int BuyerId { get; private set; }

        public Buyer Buyer { get; private set; }

        public DateTime OrderDate { get; private set; }

        public int StatusId { get; private set; }

        public OrderStatus Status { get; private set; }

        public ICollection<OrderItem> OrderItems { get; private set; }

        public int? ShippingAddressId { get; private set; }

        public Address ShippingAddress { get; private set; }

        public int PaymentId { get; private set; }

        public Payment Payment { get; private set; }

        protected Order() { }

        public Order(int buyerId, int paymentId)
        {
            BuyerId = buyerId;
            PaymentId = paymentId;
            StatusId = OrderStatus.InProcess.Id;
            OrderDate = DateTime.UtcNow;
            OrderItems = new List<OrderItem>();
        }

        public void SetAddress(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            ShippingAddress = address;
        }

        public void AddOrderItem(OrderItem item)
        {
            // Note: Some logic could be added here (like grouping items in one single OrderItem)
            // Also validation logic could be added here (like ensuring adding almost one item)
            OrderItems.Add(item);
        }
    }
}
