using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Polly.CircuitBreaker;
using WebMVC.Models;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private IOrderingService _orderSvc;
        private IBasketService _basketSvc;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        public OrderController(IOrderingService orderSvc, IBasketService basketSvc, IIdentityParser<ApplicationUser> appUserParser)
        {
            _appUserParser = appUserParser;
            _orderSvc = orderSvc;
            _basketSvc = basketSvc;
        }

        public async Task<IActionResult> Create()
        {

            var user = _appUserParser.Parse(HttpContext.User);
            var order = await _basketSvc.GetOrderDraft(user.Id);
            var vm = _orderSvc.MapUserInfoIntoOrder(user, order);
            vm.CardExpirationShortFormat();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    var basket = _orderSvc.MapOrderToBasket(model);

                    await _basketSvc.Checkout(basket);

                    //Redirect to historic list.
                    return RedirectToAction("Index");
                }
            }
            catch(BrokenCircuitException)
            {
                ModelState.AddModelError("Error", "It was not possible to create a new order, please try later on. (Business Msg Due to Circuit-Breaker)");
            }
            return View("Create",  model);
        }

        public async Task<IActionResult> Cancel(string orderId)
        {
            await _orderSvc.CancelOrder(orderId);

            //Redirect to historic list.
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(string orderId)
        {
            var user = _appUserParser.Parse(HttpContext.User);

            var order = await _orderSvc.GetOrder(user, orderId);
            return View(order);
        }

        public async Task<IActionResult> Index(Order item)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var vm = await _orderSvc.GetMyOrders(user);
            return View(vm);
        }
    }
}