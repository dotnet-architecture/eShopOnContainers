using System;
using System.Collections.Generic;

namespace eShopOnContainers.Core.Models.Orders
{
    public class Order
    {
        public int SequenceNumber { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public User.User ShippingAddress { get; set; }
        public int BuyerId { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public string OrderNumber
        {
            get { return string.Format("{0}/{1}-{2}", OrderDate.Year, OrderDate.Month, SequenceNumber); }
        }
    }
}