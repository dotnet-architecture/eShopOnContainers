using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Coupon.API.Infrastructure
{
    public class CouponRepository: ICouponRepository
    {
        private readonly IMongoCollection<Models.Coupon> _couponsCollection;

        public CouponRepository()
        {
            var mongoClient = new MongoClient("mongodb://nosqldata:27017");

            var mongoDatabase = mongoClient.GetDatabase("eshop");

            _couponsCollection = mongoDatabase.GetCollection<Models.Coupon>("coupons");
        }

        public async Task<Models.Coupon> FindByCodeAsync(string code) => 
            await _couponsCollection
                    .Find(x => string.Equals(x.Code, code))
                    .FirstOrDefaultAsync();

        public async Task AddAsync(Models.Coupon coupon) =>
            await _couponsCollection.InsertOneAsync(coupon);

        public async Task UpdateAsync(Models.Coupon updatedCoupon) =>
            await _couponsCollection.ReplaceOneAsync(x => x.Id == updatedCoupon.Id, updatedCoupon);
    }
}
