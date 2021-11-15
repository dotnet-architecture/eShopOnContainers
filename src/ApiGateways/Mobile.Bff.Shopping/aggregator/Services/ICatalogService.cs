namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services;

public interface ICatalogService
{
    Task<CatalogItem> GetCatalogItemAsync(int id);

    Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids);
}
