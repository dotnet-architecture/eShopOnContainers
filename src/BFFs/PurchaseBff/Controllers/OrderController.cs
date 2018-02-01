using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBff.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseBff.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IBasketService _basketService;
        public OrderController(IBasketService basketService)
        {
            _basketService = basketService;
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

            var order = _basketService.MapBasketToOrder(basket, isDraft: true);
            return Ok(order);
        }
    }
}
