namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services;

public interface ICatalogService
{
    Task<CatalogItem> GetCatalogItemAsync(int id);

    Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids);
}
