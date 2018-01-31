using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBff.Models;
using PurchaseBff.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseBff.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class BasketController : Controller
    {
        private readonly ICatalogService _catalog;
        private readonly IBasketService _basket;
        public BasketController(ICatalogService catalogService, IBasketService basketService)
        {
            _catalog = catalogService;
            _basket = basketService;
        }

        [HttpPut]
        [Route("items")]
        public async Task<IActionResult> UpdateQuantities([FromBody] UpdateBasketItemsRequest data)
        {
            if (!data.Updates.Any())
            {
                return BadRequest("No updates sent");
            }

            // Retrieve the current basket
            var currentBasket = await _basket.GetById(data.BasketId);
            if (currentBasket == null)
            {
                return BadRequest($"Basket with id {data.BasketId} not found.");
            }

            // Update with new quantities
            foreach (var update in data.Updates)
            {
                var basketItem = currentBasket.Items.SingleOrDefault(bitem => bitem.Id == update.BasketItemId);
                if (basketItem == null)
                {
                    return BadRequest($"Basket item with id {update.BasketItemId} not found");
                }
                basketItem.Quantity = update.NewQty;
            }

            // Save the updated basket
            await _basket.Update(currentBasket);
            return Ok(currentBasket);
        }

        [HttpPost]
        [Route("items")]
        public async Task<IActionResult> AddBasketItem([FromBody] AddBasketItemRequest data)
        {
            if (data == null || data.Quantity == 0)
            {
                return BadRequest("Invalid payload");
            }

            // Step 1: Get the item from catalog
            var item = await _catalog.GetCatalogItem(data.CatalogItemId);
            // Step 2: Get current basket status
            var currentBasket = (await _basket.GetById(data.BasketId)) ?? new BasketData(data.BasketId);
            // Step 3: Merge current status with new product
            currentBasket.Items.Add(new BasketDataItem()
            {
                UnitPrice = item.Price,
                PictureUrl = item.PictureUri,
                ProductId = item.Id.ToString(),
                ProductName = item.Name,
                Quantity = data.Quantity,
                Id = Guid.NewGuid().ToString()
            });

            // Step 4: Update basket
            await _basket.Update(currentBasket);


            return Ok();
        }
    }
}
