namespace Microsoft.eShopOnContainers.Services.Catalog.API.Model
{
    public static class CatalogItemExtensions
    {
        public static void FillProductUrl(this CatalogItem item, string picBaseUrl, bool azureStorageEnabled)
        {
            if (item != null)
            {
                item.PictureUri = azureStorageEnabled
                   ? picBaseUrl + item.PictureFileName
                   : picBaseUrl.Replace("[0]", item.Id.ToString());
            }
        }
    }
}
