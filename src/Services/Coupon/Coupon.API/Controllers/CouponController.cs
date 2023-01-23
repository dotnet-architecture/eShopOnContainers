using Microsoft.AspNetCore.Mvc;

namespace Coupon.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class CouponController : ControllerBase
{
    [HttpGet("{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetCouponByCodeAsync(string code)
    {
        await Task.Delay(500);
        
        return Ok(code);
    }
}