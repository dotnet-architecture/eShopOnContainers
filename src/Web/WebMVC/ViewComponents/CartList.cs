using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
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

        public CartList(IBasketService cartSvc) => _cartSvc = cartSvc;

        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user)
        {
            var item = await GetItemsAsync(user);
            return View(item);
        }

        // Notice that this method is a Task
        // returning asynchronous method. But, it does not
        // have the 'async' modifier, and does not contain
        // any 'await statements. 
        // The only asynchronous call is the last (or only)
        // statement of the method. In those instances,
        // a Task returning method that does not use the 
        // async modifier is preferred. The compiler generates
        // synchronous code for this method, but returns the 
        // task from the underlying asynchronous method. The
        // generated code does not contain the state machine
        // generated for asynchronous methods.
        // Contrast that with the method above, which calls
        // and awaits an asynchronous method, and then processes
        // it further.
        private Task<Basket> GetItemsAsync(ApplicationUser user) => _cartSvc.GetBasket(user);
    }
}
