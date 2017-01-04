using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.eShopOnContainers.WebMVC.Models.CartViewModels;
using Microsoft.eShopOnContainers.WebMVC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.ViewComponents
{
    public class Cart : ViewComponent
    {
        private readonly IBasketService _cartSvc;

        public Cart(IBasketService cartSvc)
        {
            _cartSvc = cartSvc;
        }

        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user)
        {
            var itemsInCart = await ItemsInCartAsync(user);
            var vm = new CartComponentViewModel()
            {
                ItemsCount = itemsInCart
            };
            return View(vm);
        }
        private async Task<int> ItemsInCartAsync(ApplicationUser user)
        {
            var basket = await _cartSvc.GetBasket(user);
            return basket.Items.Count;
        }
    }
}
