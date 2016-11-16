using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.eShopOnContainers.WebMVC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.ViewComponents
{
    public class CartList : ViewComponent
    {
        private readonly IBasketService _cartSvc;

        public CartList(IBasketService cartSvc)
        {
            _cartSvc = cartSvc;
        }

        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user)
        {
            var item = await GetItemsAsync(user);
            return View(item);
        }
        private async Task<Basket> GetItemsAsync(ApplicationUser user)
        {
            return await _cartSvc.GetBasket(user);
        }
    }
}
