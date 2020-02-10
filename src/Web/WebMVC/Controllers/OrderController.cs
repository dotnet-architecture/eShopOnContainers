using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.eShopOnContainers.WebMVC.ViewModels.Customisation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private IOrderingService _orderSvc;
        private IBasketService _basketSvc;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private static String tenantACustomisationsUrl = @"http://tenantacustomisation/";
        private static String tenantAShippingInformationUrl = @"http://tenantashippinginformation/";
        private static String tenantBShippingInformationUrl = @"http://tenantbshippinginformation/";
        private readonly ILogger<OrderController> _logger;


        public OrderController(IOrderingService orderSvc, IBasketService basketSvc,
            IIdentityParser<ApplicationUser> appUserParser, ILogger<OrderController> logger)
        {
            _appUserParser = appUserParser;
            _orderSvc = orderSvc;
            _basketSvc = basketSvc;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                ModelState.AddModelError("Error",
                    "It was not possible to create a new order, please try later on. (Business Msg Due to Circuit-Breaker)");
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
            if (user.TenantId == 1)
            {
                Boolean RFIDScanned = await AllGoodsRFIDScanned(orderId);
                ViewData["RFIDScanned"] = RFIDScanned;
            }

            var order = await _orderSvc.GetOrder(user, orderId);
            return View(order);
        }

        public async Task<IActionResult> Index(Order item)
        {
            var user = _appUserParser.Parse(HttpContext.User);

            var vm = await _orderSvc.GetMyOrders(user);

            if (user.TenantId == 1)
            {
                List<ShippingInformation> shippingInformation = GetShippingInfo(vm, tenantAShippingInformationUrl);
                _logger.LogInformation("----- Shipping info{@ShippingInformation}", shippingInformation);
                ViewData["ShippingInfo"] = shippingInformation;
            }
            else if (user.TenantId == 2)
            {
                List<ShippingInformation> shippingInformation = GetShippingInfo(vm, tenantBShippingInformationUrl);
                _logger.LogInformation("----- Shipping info{@ShippingInformation}", shippingInformation);
                ViewData["ShippingInfo"] = shippingInformation;
            }

            ViewData["TenantID"] = user.TenantId;
            return View(vm);
        }


        private async Task<Boolean> AllGoodsRFIDScanned(String orderId)
        {
            var builder = new UriBuilder(tenantACustomisationsUrl + "api/SavedEvents");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["orderId"] = orderId;
            builder.Query = query.ToString();
            string url = builder.ToString();

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(
                    url);
                if (response.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    return true;
                }

                return false;
            }
        }

        private List<ShippingInformation> GetShippingInfo(List<Order> orders, String url)
        {
            List<ShippingInformation> shippingInformation = new List<ShippingInformation>();
            using (var client = new HttpClient(new HttpClientHandler
                {AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate}))
            {
                client.BaseAddress = new Uri(url);
                try
                {
                    HttpResponseMessage response = client.GetAsync("api/shippinginformations").Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    _logger.LogInformation("----- Result{@result} -----", result);

                    List<ShippingInformation>
                        results = JsonConvert.DeserializeObject<List<ShippingInformation>>(result);
                    results.ForEach(s =>
                    {
                        if (orders.Any(item => item.OrderNumber.Equals(s.OrderNumber)))
                        {
                            shippingInformation.Add(s);
                        }
                    });
                }
                catch (Exception e)
                {
                    _logger.LogInformation("----- Exception{@e} -----", e);
                }
            }

            return shippingInformation;
        }
    }
}