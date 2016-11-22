namespace Microsoft.eShopOnContainers.Services.Ordering.Domain
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using System;
    using System.Collections.Generic;

    public class Order
        :Entity, IAggregateRoot
    {
        public int BuyerId { get; private set; }

        public Buyer Buyer { get; private set; }

        public DateTime OrderDate { get; private set; }

        public int StatusId { get; private set; }

        public OrderStatus Status { get; private set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public int? ShippingAddressId { get; private set; }

        public Address ShippingAddress { get; private set; }

        public int? BillingAddressId { get; private set; }

        public Address BillingAddress { get; private set; }

        public int PaymentId { get; private set; }

        public Payment Payment { get; private set; }

        protected Order() { }
    }
}
