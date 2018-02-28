using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IOrderApiClient _orderClient;
        public OrderController(IBasketService basketService, IOrderApiClient orderClient)
        {
            _basketService = basketService;
            _orderClient = orderClient;
        }

        [Route("draft/{basketId}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderDraft(string basketId)
        {
            if (string.IsNullOrEmpty(basketId))
            {
                return BadRequest("Need a valid basketid");
            }
            // Get the basket data and build a order draft based on it
            var basket = await _basketService.GetById(basketId);
            if (basket == null)
            {
                return BadRequest($"No basket found for id {basketId}");
            }

            var orderDraft = await _orderClient.GetOrderDraftFromBasket(basket);
            return Ok(orderDraft);
        }
    }
}
