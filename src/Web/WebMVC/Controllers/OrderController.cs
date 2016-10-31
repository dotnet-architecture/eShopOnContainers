using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private IOrderingService _orderSvc;
        private ICatalogService _catalogSvc;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderController(IOrderingService orderSvc, ICatalogService catalogSvc, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _orderSvc = orderSvc;
            _catalogSvc = catalogSvc;
        }

        public async Task<IActionResult> AddToCart(string productId)
        {
            //CCE: I need product details (price, ...), so I retrieve from Catalog service again.
            //     I don't like POST with a form from the catalog view (I'm avoiding Ajax too), 
            //     I prefer Url.Action and http call here to catalog service to retrieve this info. 
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var productDetails = _catalogSvc.GetCatalogItem(productId);
            var product = new OrderItem()
            {
                ProductId = productId, 
                Quantity = 1, 
                ProductName = productDetails.Name, 
                PicsUrl = productDetails.PicsUrl, 
                UnitPrice = productDetails.Price
            };
            _orderSvc.AddToCart(user, product);
            return RedirectToAction("Index", "Catalog");
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Index(Order item)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(_orderSvc.GetMyOrders(user));
        }
    }
}