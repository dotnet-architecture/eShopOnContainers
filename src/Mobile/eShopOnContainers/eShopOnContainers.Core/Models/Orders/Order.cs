using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace eShopOnContainers.Core.Models.Orders
{
    public class Order
    {
        public Order()
        {
            SequenceNumber = 1;
            OrderItems = new List<OrderItem>();
        }

        public string BuyerId { get; set; }

        public int SequenceNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderState State { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingStreet { get; set; }

        public string ShippingState { get; set; }

        public string ShippingCountry { get; set; }

        public string CardType { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public DateTime CardExpiration { get; set; }

        public string CardSecurityNumber { get; set; }

        [JsonProperty("items")]
        public List<OrderItem> OrderItems { get; set; }

        public decimal Total { get { return CalculateTotal(); } }

        public string OrderNumber { get { return CalculateOrderNumber(); } }


        private decimal CalculateTotal()
        {
            return OrderItems.Sum(x => x.Quantity * x.UnitPrice);
        }

        private string CalculateOrderNumber()
        {
            return string.Format("{0}/{1}-{2}", OrderDate.Year, OrderDate.Month, SequenceNumber);
        }
    }
}