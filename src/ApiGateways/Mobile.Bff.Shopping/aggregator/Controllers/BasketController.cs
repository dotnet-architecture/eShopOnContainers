using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Controllers
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

		[HttpPost]
		[HttpPut]
		public async Task<IActionResult> UpdateAllBasket([FromBody] UpdateBasketRequest data)
		{

			if (data.Items == null || !data.Items.Any())
			{
				return BadRequest("Need to pass at least one basket line");
			}

			// Retrieve the current basket
			var currentBasket = await _basket.GetById(data.BuyerId);
			if (currentBasket == null)
			{
				currentBasket = new BasketData(data.BuyerId);
			}

			System.Collections.Generic.IEnumerable<CatalogItem> catalogItems = await _catalog.GetCatalogItems(data.Items.Select(x => x.ProductId));
			BasketData newBasket = new BasketData(data.BuyerId);

			foreach (UpdateBasketRequestItemData bitem in data.Items)
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

			await _basket.Update(newBasket);
			return Ok(newBasket);
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
			BasketData currentBasket = await _basket.GetById(data.BasketId);
			if (currentBasket == null)
			{
				return BadRequest($"Basket with id {data.BasketId} not found.");
			}

			// Update with new quantities
			foreach (UpdateBasketItemData update in data.Updates)
			{
				BasketDataItem basketItem = currentBasket.Items.SingleOrDefault(bitem => bitem.Id == update.BasketItemId);
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

			//item.PictureUri = 

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
