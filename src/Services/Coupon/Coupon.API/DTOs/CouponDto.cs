namespace Coupon.API.DTOs
{
    public record CouponDto
    {
        public int Discount { get; init; }

        public string Code { get; init; }

        public bool Consumed { get; init; }
    }
}
