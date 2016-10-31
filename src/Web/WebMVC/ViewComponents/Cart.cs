using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.eShopOnContainers.WebMVC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.ViewComponents
{
    public class Cart : ViewComponent
    {
        private readonly IOrderingService _cartSvc;

        public Cart(IOrderingService cartSvc)
        {
            _cartSvc = cartSvc;
        }

        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user)
        {
            var itemsInCart = await ItemsInCartAsync(user);
            return View(itemsInCart);
        }
        private Task<int> ItemsInCartAsync(ApplicationUser user)
        {
            _cartSvc.GetActiveOrder(user);
            return Task.Run ( ()=> { return _cartSvc.ItemsInCart; });
        }
    }
}
