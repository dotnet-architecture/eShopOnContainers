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
    public class OrderController : ControllerBase
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
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(OrderData), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrderData>> GetOrderDraftAsync(string basketId)
        {
            if (string.IsNullOrEmpty(basketId))
            {
                return BadRequest("Need a valid basketid");
            }
            // Get the basket data and build a order draft based on it
            var basket = await _basketService.GetByIdAsync(basketId);

            if (basket == null)
            {
                return BadRequest($"No basket found for id {basketId}");
            }

            return await _orderClient.GetOrderDraftFromBasketAsync(basket);
        }
    }
}
