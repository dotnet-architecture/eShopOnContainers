using System;
using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries
{
    public class Orderitem
    {
        public string productname { get; set; }
        public int units { get; set; }
        public double unitprice { get; set; }
        public string pictureurl { get; set; }
    }

    public class Order
    {
        public int ordernumber { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public string country { get; set; }
        public List<Orderitem> orderitems { get; set; }
        public decimal total { get; set; }
    }

    public class OrderSummary
    {
        public int ordernumber { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public double total { get; set; }
    }

    public class CardType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
