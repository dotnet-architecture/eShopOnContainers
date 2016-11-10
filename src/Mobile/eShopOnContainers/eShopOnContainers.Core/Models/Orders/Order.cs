using System;

namespace eShopOnContainers.Core.Models.Orders
{
    public class Order
    {
        public long OrderNumber { get; set; }
        public double Total { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }
    }
}
