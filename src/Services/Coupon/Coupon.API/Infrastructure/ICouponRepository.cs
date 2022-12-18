namespace Coupon.API.Infrastructure
{
    public interface ICouponRepository
    {
        Task<Models.Coupon> FindByCodeAsync(string code);

        Task AddAsync(Models.Coupon coupon);

        Task UpdateAsync(Models.Coupon updatedCoupon);
    }
}
