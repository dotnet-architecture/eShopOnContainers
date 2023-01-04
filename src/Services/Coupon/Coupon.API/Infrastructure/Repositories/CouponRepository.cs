namespace Coupon.API.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Coupon.API.Infrastructure.Models;
    using MongoDB.Driver;

    public class CouponRepository : ICouponRepository
    {
        private readonly CouponContext _couponContext;

        public CouponRepository(CouponContext couponContext)
        {
            _couponContext = couponContext;
        }

        public async Task UpdateCouponConsumedByCodeAsync(string code, int orderId)
        {
            var filter = Builders<Coupon>.Filter.Eq("Code", code);
            var update = Builders<Coupon>.Update
                .Set(coupon => coupon.Consumed, true)
                .Set(coupon => coupon.OrderId, orderId);

            await _couponContext.Coupons.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }

        public async Task UpdateCouponReleasedByOrderIdAsync(int orderId)
        {
            var filter = Builders<Coupon>.Filter.Eq("OrderId", orderId);
            var update = Builders<Coupon>.Update
                .Set(coupon => coupon.Consumed, false)
                .Set(coupon => coupon.OrderId, 0);

            await _couponContext.Coupons.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }

        public async Task<Coupon> FindCouponByCodeAsync(string code)
        {
            var filter = Builders<Coupon>.Filter.Eq("Code", code);
            return await _couponContext.Coupons.Find(filter).FirstOrDefaultAsync();
        }
    }
}
