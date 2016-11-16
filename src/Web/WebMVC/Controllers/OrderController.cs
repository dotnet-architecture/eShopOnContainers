using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.eShopOnContainers.WebMVC.Models.OrderViewModels;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private IOrderingService _orderSvc;
        private IBasketService _basketSvc;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderController(IOrderingService orderSvc, IBasketService basketSvc, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _orderSvc = orderSvc;
            _basketSvc = basketSvc;
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CreateOrderViewModel();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var basket = await _basketSvc.GetBasket(user);
            var order = _basketSvc.MapBasketToOrder(basket);
            vm.Order = _orderSvc.MapUserInfoIntoOrder(user, order);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderViewModel model, Dictionary<string, int> quantities, string action)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var basket = await _basketSvc.SetQuantities(user, quantities);
            basket = await _basketSvc.UpdateBasket(basket);
            var order = _basketSvc.MapBasketToOrder(basket);

            // override if user has changed some shipping address or payment info data.
            _orderSvc.OverrideUserInfoIntoOrder(model.Order, order);

            if (action == "[ Place Order ]")
            {
                _orderSvc.CreateOrder(user, order);

                //Empty basket for current user. 
                await _basketSvc.CleanBasket(user);

                //Redirect to historic list.
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Detail(string orderId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var order = _orderSvc.GetOrder(user, orderId);

            return View(order);
        }

        public async Task<IActionResult> Index(Order item)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(_orderSvc.GetMyOrders(user));
        }
    }
}