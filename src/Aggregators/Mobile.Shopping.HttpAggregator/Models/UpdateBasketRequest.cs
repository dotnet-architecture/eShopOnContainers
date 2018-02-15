using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models
{
    public class UpdateBasketRequest
    {
        public string BuyerId { get; set; }

        public IEnumerable<UpdateBasketRequestItemData> Items { get; set; }
    }

    public class UpdateBasketRequestItemData
    {
        public string Id { get; set; }          // Basket id
        public int ProductId { get; set; }      // Catalog item id
        public int Quantity { get; set; }       // Quantity
    }
}
