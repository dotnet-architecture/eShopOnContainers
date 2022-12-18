using Coupon.API.DTOs;
using Coupon.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Coupon.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CouponController: ControllerBase
    {
        private ICouponRepository _couponRepository;

        public CouponController(ICouponRepository couponeRepository) =>
            _couponRepository = couponeRepository;

        [HttpGet("value/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CouponDto>> GetCouponByCodeAsync(string code)
        {
            var coupon = await _couponRepository.FindByCodeAsync(code);

            if (coupon is null || coupon.Consumed)
            {
                return NotFound();
            }

            return new CouponDto
            {
                Code = code,
                Consumed = coupon.Consumed,
            };
        }
    }
}
