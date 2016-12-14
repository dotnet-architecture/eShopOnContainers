using System;

namespace eShopOnContainers.Core.Models.Basket
{
    public class BasketItem 
    {
        public string Id { get; set; }
       
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public string PictureUrl { get; set; }

        public decimal Total { get { return Quantity * UnitPrice; } }

        public override string ToString()
        {
            return String.Format("Product Id: {0}, Quantity: {1}", ProductId, Quantity);
        }
    }
}