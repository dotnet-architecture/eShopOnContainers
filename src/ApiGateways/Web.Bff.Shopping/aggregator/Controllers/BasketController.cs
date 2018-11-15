using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ICatalogService _catalog;
        private readonly IBasketService _basket;

        public BasketController(ICatalogService catalogService, IBasketService basketService)
        {
            _catalog = catalogService;
            _basket = basketService;
        }

        [HttpPost]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BasketData), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketData>> UpdateAllBasketAsync([FromBody] UpdateBasketRequest data)
        {
            if (data.Items == null || !data.Items.Any())
            {
                return BadRequest("Need to pass at least one basket line");
            }

            // Retrieve the current basket
            var currentBasket = await _basket.GetByIdAsync(data.BuyerId);
            if (currentBasket == null)
            {
                currentBasket = new BasketData(data.BuyerId);
            }

            var catalogItems = await _catalog.GetCatalogItemsAsync(data.Items.Select(x => x.ProductId));
            var newBasket = new BasketData(data.BuyerId);

            foreach (var bitem in data.Items)
            {
                var catalogItem = catalogItems.SingleOrDefault(ci => ci.Id == bitem.ProductId);
                if (catalogItem == null)
                {
                    return BadRequest($"Basket refers to a non-existing catalog item ({bitem.ProductId})");
                }

                newBasket.Items.Add(new BasketDataItem()
                {
                    Id = bitem.Id,
                    ProductId = catalogItem.Id.ToString(),
                    ProductName = catalogItem.Name,
                    PictureUrl = catalogItem.PictureUri,
                    UnitPrice = catalogItem.Price,
                    Quantity = bitem.Quantity
                });
            }

            await _basket.UpdateAsync(newBasket);

            return newBasket;
        }

        [HttpPut]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BasketData), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketData>> UpdateQuantitiesAsync([FromBody] UpdateBasketItemsRequest data)
        {
            if (!data.Updates.Any())
            {
                return BadRequest("No updates sent");
            }

            // Retrieve the current basket
            var currentBasket = await _basket.GetByIdAsync(data.BasketId);
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
            await _basket.UpdateAsync(currentBasket);

            return currentBasket;
        }

        [HttpPost]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddBasketItemAsync([FromBody] AddBasketItemRequest data)
        {
            if (data == null || data.Quantity == 0)
            {
                return BadRequest("Invalid payload");
            }

            // Step 1: Get the item from catalog
            var item = await _catalog.GetCatalogItemAsync(data.CatalogItemId);

            //item.PictureUri = 

            // Step 2: Get current basket status
            var currentBasket = (await _basket.GetByIdAsync(data.BasketId)) ?? new BasketData(data.BasketId);
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
            await _basket.UpdateAsync(currentBasket);

            return Ok();
        }
    }
}
