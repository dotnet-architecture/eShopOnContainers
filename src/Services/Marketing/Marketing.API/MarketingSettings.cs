namespace Microsoft.eShopOnContainers.Services.Marketing.API
{
    public class MarketingSettings
    {
        public string ConnectionString { get; set; }
        public string MongoConnectionString { get; set; }
        public string MongoDatabase { get; set; }
        public string ExternalCatalogBaseUrl { get; set; }
        public string CampaignDetailFunctionUri { get; set; }
        public string PicBaseUrl { get; set; }
        public bool AzureStorageEnabled { get; set; }
    }
}
