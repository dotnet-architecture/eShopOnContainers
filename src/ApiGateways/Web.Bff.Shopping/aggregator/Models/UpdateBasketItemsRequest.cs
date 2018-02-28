using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models
{
    public class UpdateBasketItemsRequest
    {
        
        public string BasketId { get; set; }

        public ICollection<UpdateBasketItemData> Updates { get; set; }

        public UpdateBasketItemsRequest()
        {
            Updates = new List<UpdateBasketItemData>();
        }
    }

    public class UpdateBasketItemData
    {
        public string BasketItemId { get; set; }
        public int NewQty { get; set; }

        public UpdateBasketItemData()
        {
            NewQty = 0;
        }
    }
}
