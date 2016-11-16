using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Models
{
    public class Basket
    {
        public Basket()
        {
            Items = new List<BasketItem>();
        }
        public List<BasketItem> Items { get; set; }
        public string BuyerId { get; set; }

        public decimal Total()
        {
            return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity),2);
        }
    }
}
