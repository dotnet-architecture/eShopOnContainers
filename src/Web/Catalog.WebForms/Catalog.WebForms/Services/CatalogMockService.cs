using eShopOnContainers.Core.Extensions;
using eShopOnContainers.Core.Models.Catalog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Catalog
{

    // From https://github.com/dotnet/eShopOnContainers/blob/vs2017/src/Mobile/eShopOnContainers/eShopOnContainers.Core/Services/Catalog/CatalogMockService.cs
    // Issue: How to make this DRY, while preserving the story for a monolithic application.
    // Note: Device specific conditionals have been removed.
    public class CatalogMockService : ICatalogService
    {
        // Change to static so these remain 
        // in scope between requests:
        private static ObservableCollection<CatalogBrand> MockCatalogBrand = new ObservableCollection<CatalogBrand>
        {
            new CatalogBrand { Id = 1, Brand = "Azure" },
            new CatalogBrand { Id = 2, Brand = "Visual Studio" }
        };

        private static ObservableCollection<CatalogType> MockCatalogType = new ObservableCollection<CatalogType>
        {
            new CatalogType { Id = 1, Type = "Mug" },
            new CatalogType { Id = 2, Type = "T-Shirt" }
        };

        private static ObservableCollection<CatalogItem> MockCatalog = new ObservableCollection<CatalogItem>
        {
            new CatalogItem { Id = Common.Common.MockCatalogItemId01, PictureUri = "Content/fake_product_01.png", Name = ".NET Bot Blue Sweatshirt (M)", Price = 19.50M, CatalogBrandId = 2, CatalogBrand = "Visual Studio", CatalogTypeId = 2, CatalogType = "T-Shirt" },
            new CatalogItem { Id = Common.Common.MockCatalogItemId02, PictureUri = "Content/fake_product_02.png", Name = ".NET Bot Purple Sweatshirt (M)", Price = 19.50M, CatalogBrandId = 2, CatalogBrand = "Visual Studio", CatalogTypeId = 2, CatalogType = "T-Shirt" },
            new CatalogItem { Id = Common.Common.MockCatalogItemId03, PictureUri = "Content/fake_product_03.png", Name = ".NET Bot Black Sweatshirt (M)", Price = 19.95M, CatalogBrandId = 2, CatalogBrand = "Visual Studio", CatalogTypeId = 2, CatalogType = "T-Shirt" },
            new CatalogItem { Id = Common.Common.MockCatalogItemId04, PictureUri = "Content/fake_product_04.png", Name = ".NET Black Cupt", Price = 17.00M, CatalogBrandId = 2, CatalogBrand = "Visual Studio", CatalogTypeId = 1, CatalogType = "Mug" },
            new CatalogItem { Id = Common.Common.MockCatalogItemId05, PictureUri = "Content/fake_product_05.png", Name = "Azure Black Sweatshirt (M)", Price = 19.50M, CatalogBrandId = 1, CatalogBrand = "Azure", CatalogTypeId = 2, CatalogType = "T-Shirt" }
        };

        public async Task<ObservableCollection<CatalogItem>> GetCatalogAsync()
        {
            await Task.Delay(500);

            // copy so that edits must be changed here to take affect:
            return new ObservableCollection<CatalogItem>(MockCatalog);
        }

        public async Task<ObservableCollection<CatalogItem>> FilterAsync(int catalogBrandId, int catalogTypeId)
        {
            await Task.Delay(500);

            return MockCatalog
                .Where(c => c.CatalogBrandId == catalogBrandId &&
                c.CatalogTypeId == catalogTypeId)
                .ToObservableCollection();
        }

        public async Task<ObservableCollection<CatalogBrand>> GetCatalogBrandAsync()
        {
            await Task.Delay(500);

            return MockCatalogBrand;
        }

        public async Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync()
        {
            await Task.Delay(500);

            return MockCatalogType;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(string id)
        {
            await Task.Delay(500);

            return MockCatalog.FirstOrDefault(c => c.Id == id);
        }

        public async Task DeleteCatalogItemAsync(string catalogItemId)
        {
            var itemToRemove = MockCatalog.FirstOrDefault(c => c.Id == catalogItemId);
            await Task.Delay(500);
            if (itemToRemove != null)
                MockCatalog.Remove(itemToRemove);
            else
                throw new ArgumentException(message: "item could not be found", paramName: nameof(catalogItemId));
        }

        public async Task<CatalogItem> UpdateCatalogItemAsync(CatalogItem item)
        {
            var itemToChange = MockCatalog.FirstOrDefault(c => item.Id == c.Id);
            await Task.Delay(500);
            if (itemToChange != null)
            {
                itemToChange.CatalogBrandId = item.CatalogBrandId;
                itemToChange.CatalogTypeId = item.CatalogTypeId;
                itemToChange.Description = item.Description;
                itemToChange.Name = item.Name;
                itemToChange.Price = item.Price;
            }
            return itemToChange;
        }

    }
}