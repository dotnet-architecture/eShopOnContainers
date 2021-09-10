namespace Microsoft.eShopOnContainers.Services.Catalog.API.Model
{
    public static class CatalogItemExtensions
    {
        public static void FillProductUrl(this CatalogItem item, string picBaseUrl, bool azureStorageEnabled)
        {
            if (item != null)
            {
                // https://www.linkedin.com/feed/update/urn:li:activity:6841055191211491328/
                item.PictureUri = azureStorageEnabled switch
                {
                    true => picBaseUrl + item.PictureFileName,
                    _ => picBaseUrl.Replace("[0]", item.Id.ToString())
                };
            }
        }
    }
}
