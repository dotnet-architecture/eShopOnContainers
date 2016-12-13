using eShopOnContainers.Core.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eShopOnContainers.Core.Models.Orders
{
    public class Order
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
            ShippingAddress = new Address();
            PaymentInfo = new PaymentInfo();
        }

        public string Id;
        public List<OrderItem> OrderItems { get; set; }

        public string OrderNumber
        {
            get
            {
                return string.Format("{0}/{1}-{2}", OrderDate.Year, OrderDate.Month, SequenceNumber);
            }
        }
        public int SequenceNumber { get; set; }
        public virtual string BuyerId { get; set; }
        public virtual Address ShippingAddress { get; set; }
        public virtual PaymentInfo PaymentInfo { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public OrderState State { get; set; }

        public decimal Total { get { return CalculateTotal(); } }


        public decimal CalculateTotal()
        {
            return OrderItems.Sum(x => x.Quantity * x.UnitPrice);
        }
    }
}