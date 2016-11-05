using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBasketService _basketSvc;
        private readonly ICatalogService _catalogSvc;

        public CartController(IBasketService basketSvc, ICatalogService catalogSvc, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _basketSvc = basketSvc;
            _catalogSvc = catalogSvc;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var vm = _basketSvc.GetBasket(user);

            return View(vm);
        }

        
        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> quantities, string action)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var basket = _basketSvc.SetQuantities(user, quantities);
            var vm = _basketSvc.UpdateBasket(basket);

            if (action == "[ Checkout ]")
            {
                var order = _basketSvc.MapBasketToOrder(basket);
                return RedirectToAction("Create", "Order");
            }
           
            return View(vm);
        }

        public async Task<IActionResult> AddToCart(string productId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var productDetails = _catalogSvc.GetCatalogItem(productId);
            var product = new BasketItem()
            {
                Id = Guid.NewGuid().ToString(),
                Quantity = 1,
                ProductName = productDetails.Name,
                PictureUrl = productDetails.PictureUri,
                UnitPrice = productDetails.Price, 
                ProductId = productId
            };
            _basketSvc.AddItemToBasket(user, product);
            return RedirectToAction("Index", "Catalog");
        }
    }
}