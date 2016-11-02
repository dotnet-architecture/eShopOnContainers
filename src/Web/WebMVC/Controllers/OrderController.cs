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
            var basket = _basketSvc.GetBasket(user);
            var order = _basketSvc.MapBasketToOrder(basket);
            vm.Order = order;
            
            return View(vm);
        }

        public async Task<IActionResult> Index(Order item)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(_orderSvc.GetMyOrders(user));
        }
    }
}