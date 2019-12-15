using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IOrderingService _orderingService;

        public OrderController(IBasketService basketService, IOrderingService orderingService)
        {
            _basketService = basketService;
            _orderingService = orderingService;
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
            var basket = await _basketService.GetById(basketId);

            if (basket == null)
            {
                return BadRequest($"No basket found for id {basketId}");
            }

            return await _orderingService.GetOrderDraftAsync(basket);
        }
    }
}
