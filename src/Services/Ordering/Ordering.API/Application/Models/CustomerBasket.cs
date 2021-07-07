using System.Collections.Generic;

namespace Ordering.API.Application.Models
{
    public class CustomerBasket
    {
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; }

        public CustomerBasket(string buyerId, List<BasketItem> items)
        {
            BuyerId = buyerId;
            Items = items;
        }
    }
}
