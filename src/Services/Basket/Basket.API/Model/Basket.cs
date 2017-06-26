using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public class CustomerBasket
    {
        public string BuyerId { get;  set; }
        public List<BasketItem> Items { get; set; } 

        public CustomerBasket(string customerId)
        {
            BuyerId = customerId;
            Items = new List<BasketItem>();
        }
    }
}
