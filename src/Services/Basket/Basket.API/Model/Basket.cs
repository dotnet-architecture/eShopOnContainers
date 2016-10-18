using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public class CustomerBasket
    {
        public Guid CustomerId { get; private set; }
        public IList<BasketItem> BasketItems => new List<BasketItem>();

        public CustomerBasket(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
