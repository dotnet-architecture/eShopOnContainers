namespace Microsoft.eShopOnContainers.Services.Catalog.API
{
    public class CatalogSettings
    {
        public string ExternalCatalogBaseUrl {get;set;}

        public string EventBusConnection { get; set; }

        public bool UseCustomizationData { get; set; }
    }
}
