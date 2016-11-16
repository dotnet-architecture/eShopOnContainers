using Microsoft.eShopOnContainers.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface IBasketService
    {
        Task<Basket> GetBasket(ApplicationUser user);
        Task AddItemToBasket(ApplicationUser user, BasketItem product);
        Task<Basket> UpdateBasket(Basket basket);
        Task<Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities);
        Order MapBasketToOrder(Basket basket);
        Task CleanBasket(ApplicationUser user);
    }
}
