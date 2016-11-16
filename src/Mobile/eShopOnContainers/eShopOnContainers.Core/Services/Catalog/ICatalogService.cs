using eShopOnContainers.Core.Models.Catalog;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Catalog
{
    public interface ICatalogService
    {
        Task<ObservableCollection<CatalogItem>> GetCatalogAsync();
        Task<CatalogItem> GetCatalogItemAsync(string id);
    }
}
