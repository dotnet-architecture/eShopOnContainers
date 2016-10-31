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
        private readonly ICartService _cartSvc;

        public CartList(ICartService cartSvc)
        {
            _cartSvc = cartSvc;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var item = await GetItemsAsync();
            return View(item);
        }
        private Task<Order> GetItemsAsync()
        {
            return _cartSvc.GetActiveOrder();
        }
    }
}
