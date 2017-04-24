using Newtonsoft.Json;
using System;

namespace eShopOnContainers.Core.Models.Orders
{
    public class OrderItem
    {
        public string ProductId { get; set; }
        public Guid? OrderId { get; set; }

        [JsonProperty("unitprice")]
        public decimal UnitPrice { get; set; }

        [JsonProperty("productname")]
        public string ProductName { get; set; }

        [JsonProperty("pictureurl")]
        public string PictureUrl { get; set; }

        [JsonProperty("units")]
        public int Quantity { get; set; }

        public decimal Discount { get; set; }
        public decimal Total { get { return Quantity * UnitPrice; } }

        public override string ToString()
        {
            return String.Format("Product Id: {0}, Quantity: {1}", ProductId, Quantity);
        }
    }
}