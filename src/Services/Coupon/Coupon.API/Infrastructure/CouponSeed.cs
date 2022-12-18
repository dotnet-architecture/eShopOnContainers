namespace Coupon.API.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Coupon.API.Infrastructure.Models;
    using Coupon.API.Infrastructure.Repositories;

    public class CouponSeed
    {
        public async Task SeedAsync(CouponContext context)
        {
            if (context.Coupons.EstimatedDocumentCount() == 0)
            {
                var coupons = new List<Coupon>
                {
                    new Coupon
                    {
                        Code = "DISC-5",
                        Discount = 5
                    },
                    new Coupon
                    {
                        Code = "DISC-10",
                        Discount = 10
                    },
                    new Coupon
                    {
                        Code = "DISC-15",
                        Discount = 15
                    },
                    new Coupon
                    {
                        Code = "DISC-20",
                        Discount = 20
                    },
                    new Coupon
                    {
                        Code = "DISC-25",
                        Discount = 25
                    },
                    new Coupon
                    {
                        Code = "DISC-30",
                        Discount = 30
                    }
                };

                await context.Coupons.InsertManyAsync(coupons);
            }
        }
    }
}
