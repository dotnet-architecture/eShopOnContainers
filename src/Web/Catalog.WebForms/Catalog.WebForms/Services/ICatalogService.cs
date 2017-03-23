using eShopOnContainers.Core.Models.Catalog;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// Taken from https://github.com/dotnet/eShopOnContainers/blob/vs2017/src/Mobile/eShopOnContainers/eShopOnContainers.Core/Services/Catalog/ICatalogService.cs
// Open Issue: How do we make this application DRY and still support the story
// of a 'monolithic' app for a 'Lift and Shift' scenario?
namespace eShopOnContainers.Core.Services.Catalog
{
    public interface ICatalogService
    {
        Task<ObservableCollection<CatalogBrand>> GetCatalogBrandAsync();
        Task<ObservableCollection<CatalogItem>> FilterAsync(int catalogBrandId, int catalogTypeId);
        Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync();
        Task<ObservableCollection<CatalogItem>> GetCatalogAsync();
        Task<CatalogItem> GetCatalogItemAsync(string id);
        Task DeleteCatalogItemAsync(string catalogItemId);
        Task<CatalogItem> UpdateCatalogItemAsync(CatalogItem item);

    }
}