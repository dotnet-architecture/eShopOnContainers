using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IBasketService _basketSvc;
        private readonly ICatalogService _catalogSvc;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        public CartController(IBasketService basketSvc, ICatalogService catalogSvc, IIdentityParser<ApplicationUser> appUserParser)
        {
            _basketSvc = basketSvc;
            _catalogSvc = catalogSvc;
            _appUserParser = appUserParser;
        }

        public async Task<IActionResult> Index()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var vm = await _basketSvc.GetBasket(user);
            

            return View(vm);
        }

        
        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> quantities, string action)
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var basket = await _basketSvc.SetQuantities(user, quantities);
            var vm = await _basketSvc.UpdateBasket(basket);

            if (action == "[ Checkout ]")
            {
                var order = _basketSvc.MapBasketToOrder(basket);
                return RedirectToAction("Create", "Order");
            }
           
            return View(vm);
        }

        public async Task<IActionResult> AddToCart(CatalogItem productDetails)
        {
            if (productDetails.Id != null)
            {
                var user = _appUserParser.Parse(HttpContext.User);
                var product = new BasketItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Quantity = 1,
                    ProductName = productDetails.Name,
                    PictureUrl = productDetails.PictureUri,
                    UnitPrice = productDetails.Price,
                    ProductId = productDetails.Id
                };
                await _basketSvc.AddItemToBasket(user, product);
            }
            return RedirectToAction("Index", "Catalog");
        }
    }
}
