namespace Microsoft.eShopOnContainers.Services.Ordering.API
{
    public class OrderingSettings
    {
        public bool UseCustomizationData { get; set; }

        public string ConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public int GracePeriodTime { get; set; }

        public int CheckUpdateTime { get; set; }
    }
}
