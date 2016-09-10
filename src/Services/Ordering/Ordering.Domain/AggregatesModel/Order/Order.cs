using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel
{
    public class Order : AggregateRoot
    {
        public Order(Guid buyerId, Address shippingAddress, Address billingAddress)
              : this(buyerId, shippingAddress, billingAddress, DateTime.UtcNow)
        {
        }

        public Order(Guid buyerId, Address shippingAddress, Address billingAddress, DateTime orderDate)
        {
            this.BuyerId = buyerId;
            this.ShippingAddress = shippingAddress;
            this.BillingAddress = billingAddress;
            this.OrderDate = orderDate;
        }

        //Infrastructure requisite - Parameterless constructor needed by EF
        Order() { }

        //Order ID comes derived from the Entity base class

        //List<OrderItem> _orderItems;
        //public virtual List<OrderItem> orderItems
        //{
        //    get
        //    {
        //        if (_orderItems == null)
        //            _orderItems = new List<OrderItem>();

        //        return _orderItems;
        //    }

        //    private set
        //    {
        //        _orderItems = value;
        //    }
        //}

        public virtual Guid BuyerId { get; private set; }

        public virtual Address ShippingAddress { get; private set; }
        
        public virtual Address BillingAddress { get; private set; }

        public virtual DateTime OrderDate { get; private set; }

        public virtual OrderStatus Status { get; private set; }

    }
}
