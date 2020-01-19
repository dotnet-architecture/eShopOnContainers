using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.eShopOnContainers.WebMVC.ViewModels.Customisation;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private IOrderingService _orderSvc;
        private IBasketService _basketSvc;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private static String url = @"http://tenantacustomisation/";

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
            catch (BrokenCircuitException)
            {
                ModelState.AddModelError("Error", "It was not possible to create a new order, please try later on. (Business Msg Due to Circuit-Breaker)");
            }

            return View("Create", model);
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
            List<ShippingInformation> shippingInformation = GetShippingInfo(vm);
            ViewData["ShippingInfo"] = shippingInformation;
            return View(vm);
        }

        private List<ShippingInformation> GetShippingInfo(List<Order> orders)
        {
            List<ShippingInformation> shippingInformation = new List<ShippingInformation>();
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(url);
                try
                {
                    HttpResponseMessage response = client.GetAsync("api/shippinginformation").Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    List<ShippingInformation> results = JsonConvert.DeserializeObject<List<ShippingInformation>>(result);
                    results.ForEach( s =>
                    {
                       if(orders.Any(item => item.OrderNumber.Equals(s.OrderNumber)))
                        {
                            shippingInformation.Add(s);
                        }
                    });

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return shippingInformation;
        }
    }
}