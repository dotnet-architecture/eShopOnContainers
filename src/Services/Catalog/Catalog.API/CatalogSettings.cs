namespace Microsoft.eShopOnContainers.Services.Catalog.API
{
    public class CatalogSettings
    {
        public string PicBaseUrl { get;set;}

        public string EventBusConnection { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public bool AzureStorageEnabled { get; set; }
    }
}
