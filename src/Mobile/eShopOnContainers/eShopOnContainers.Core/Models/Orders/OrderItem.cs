using System;

namespace eShopOnContainers.Core.Models.Orders
{
    public class OrderItem
    {
        public string ProductId { get; set; }
        public Guid OrderId { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get { return Quantity * UnitPrice; } }

        public override string ToString()
        {
            return String.Format("Product Id: {0}, Quantity: {1}", ProductId, Quantity);
        }
    }
}