using Microsoft.eShopOnContainers.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface IBasketService
    {
        Basket GetBasket(ApplicationUser user);
        int ItemsInCart { get; }
        void AddItemToBasket(ApplicationUser user, BasketItem product);
        Basket UpdateBasket(Basket basket);
        Basket SetQuantities(ApplicationUser user, Dictionary<string, int> quantities);
        Order MapBasketToOrder(Basket basket);
        void CleanBasket(ApplicationUser user);
    }
}
