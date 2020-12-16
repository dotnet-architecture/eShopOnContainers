using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public record CustomerBasket
    {
        public string BuyerId { get; init; }

        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public CustomerBasket()
        {

        }

        public CustomerBasket(string customerId)
        {
            BuyerId = customerId;
        }
    }
}
