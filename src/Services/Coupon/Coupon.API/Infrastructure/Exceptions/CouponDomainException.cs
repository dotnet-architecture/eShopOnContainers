namespace Coupon.API.Infrastructure.Exceptions
{
    public class CouponDomainException : Exception
    {
        public CouponDomainException()
        { }

        public CouponDomainException(string message)
            : base(message)
        { }

        public CouponDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
