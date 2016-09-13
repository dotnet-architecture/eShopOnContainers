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

            this.Status = OrderStatus.New;
        }

        //Infrastructure requisite - Parameterless constructor needed by EF
        Order() { }

        //Order ID comes derived from the Entity base class

        List<OrderItem> _orderItems;
        public virtual List<OrderItem> OrderItems
        {
            get
            {
                if (_orderItems == null)
                    _orderItems = new List<OrderItem>();

                return _orderItems;
            }

            private set
            {
                _orderItems = value;
            }
        }

        public string OrderNumber
        {
            get
            {
                return string.Format("{0}/{1}-{2}", OrderDate.Year, OrderDate.Month, SequenceNumber);
            }
        }

        public int SequenceNumber { get; set; }

        public virtual Guid BuyerId { get; private set; }

        public virtual Address ShippingAddress { get; private set; }
        
        public virtual Address BillingAddress { get; private set; }

        public virtual DateTime OrderDate { get; private set; }

        public virtual OrderStatus Status { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////
        //Domain Rules and Logic in Order Aggregate-Root (Sample of a "NO ANEMIC DOMAIN MODEL" )
        ////////////////////////////////////////////////////////////////////////////////////////

        public OrderItem AddNewOrderItem(Guid productId, int quantity, decimal unitPrice, decimal discount)
        {
            //check preconditions
            if (productId == Guid.Empty)
                throw new ArgumentNullException("productId");

            if (quantity <= 0)
            {
                throw new ArgumentException("The quantity of Product in an Order cannot be equal or less than cero");
            }

            //check discount values
            if (discount < 0)
                discount = 0;

            if (discount > 100)
                discount = 100;

            //create new order line
            var newOrderItem = new OrderItem()
            {
                OrderId = this.Id,
                ProductId = productId,
                Quantity = quantity,
                FulfillmentRemaining = quantity,
                Discount = discount,
                UnitPrice = unitPrice
            };

            //set identity
            newOrderItem.GenerateNewIdentity();

            //add order item
            this.OrderItems.Add(newOrderItem);

            //return added orderline
            return newOrderItem;
        }
    }
}
