namespace Coupon.API.Infrastructure
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string CouponsCollectionName { get; set; }
    }
}
