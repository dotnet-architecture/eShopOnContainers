using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models
{
    public class BasketData
    {
        public string BuyerId { get; set; }
        public List<BasketDataItem> Items { get; set; }

        public BasketData(string buyerId)
        {
            BuyerId = buyerId;
            Items = new List<BasketDataItem>();
        }
    }

    public class BasketDataItem
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OldUnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }

    }
}
