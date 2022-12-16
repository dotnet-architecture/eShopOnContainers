namespace Coupon.API
{
    public class CouponSettings
    {
        public string ConnectionString { get; set; }

        public string CouponMongoDatabase { get; set; }

        public string EventBusConnection { get; set; }

        public bool UseCustomizationData { get; set; }

        public bool AzureStorageEnabled { get; set; }
    }
}
