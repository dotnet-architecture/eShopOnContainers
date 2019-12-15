using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models
{
    public class BasketData
    {
        public string BuyerId { get; set; }

        public List<BasketDataItem> Items { get; set; } = new List<BasketDataItem>();

        public BasketData()
        {

        }

        public BasketData(string buyerId)
        {
            BuyerId = buyerId;
        }
    }

    public class BasketDataItem
    {
        public string Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OldUnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }

    }
}
