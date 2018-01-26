using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseBff.Controllers
{
    [Route("api/v1/[controller]")]
    public class BasketController : Controller
    {
        [HttpPost("items/{catalogItemId}")]
        public async Task<IActionResult> AddBasketItem(int catalogItemId)
        {
            // Step 1: Get the item from catalog
            // Step 2: Get current basket status
            // Step 3: Merge current status with new product
            // Step 4: Update basket

            return Ok();
        }
    }
}
