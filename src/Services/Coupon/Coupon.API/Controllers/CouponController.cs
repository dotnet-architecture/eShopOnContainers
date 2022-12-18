namespace Coupon.API.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using Coupon.API.Dtos;
    using Coupon.API.Infrastructure.Models;
    using Coupon.API.Infrastructure.Repositories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper<CouponDto, Coupon> _mapper;

        public CouponController(ICouponRepository couponRepository, IMapper<CouponDto, Coupon> mapper)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
        }

        // Add the GetCouponByCodeAsync method
    }
}
