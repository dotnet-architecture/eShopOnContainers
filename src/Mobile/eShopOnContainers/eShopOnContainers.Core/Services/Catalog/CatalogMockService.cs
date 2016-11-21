using eShopOnContainers.Core.Extensions;
using eShopOnContainers.Core.Models.Catalog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Services.Catalog
{
    public class CatalogMockService : ICatalogService
    {
        private ObservableCollection<CatalogBrand> MockCatalogBrand = new ObservableCollection<CatalogBrand>
        {
            new CatalogBrand { Id = 1, Brand = "Azure" },
            new CatalogBrand { Id = 2, Brand = "Visual Studio" }
        };

        private ObservableCollection<CatalogType> MockCatalogType = new ObservableCollection<CatalogType>
        {
            new CatalogType { Id = 1, Type = "Mug" },
            new CatalogType { Id = 2, Type = "T-Shirt" }
        };

        private ObservableCollection<CatalogItem> MockCatalog = new ObservableCollection<CatalogItem>
        {
            new CatalogItem { Id = "1", PictureUri = Device.OS != TargetPlatform.Windows? "fake_product_01" : "Assets/fake_product_01.png", Name = ".NET Bot Blue Sweatshirt (M)", Price = 19.50M, CatalogBrand = "Visual Studio", CatalogType = "T-Shirt" },
            new CatalogItem { Id = "2", PictureUri = Device.OS != TargetPlatform.Windows? "fake_product_02": "Assets/fake_product_02.png", Name = ".NET Bot Purple Sweatshirt (M)", Price = 19.50M, CatalogBrand = "Visual Studio", CatalogType = "T-Shirt" },
            new CatalogItem { Id = "3", PictureUri = Device.OS != TargetPlatform.Windows? "fake_product_03": "Assets/fake_product_03.png", Name = ".NET Bot Black Sweatshirt (M)", Price = 19.95M, CatalogBrand = "Visual Studio", CatalogType = "T-Shirt" },
            new CatalogItem { Id = "4", PictureUri = Device.OS != TargetPlatform.Windows? "fake_product_04": "Assets/fake_product_04.png", Name = ".NET Black Cupt", Price = 17.00M, CatalogBrand = "Visual Studio", CatalogType = "Mug" },
            new CatalogItem { Id = "5", PictureUri = Device.OS != TargetPlatform.Windows? "fake_product_05": "Assets/fake_product_05.png", Name = "Azure Black Sweatshirt (M)", Price = 19.50M, CatalogBrand = "Azure", CatalogType = "T-Shirt" }
        };

        public async Task<ObservableCollection<CatalogItem>> GetCatalogAsync()
        {
            await Task.Delay(500);

            return MockCatalog;
        }

        public async Task<ObservableCollection<CatalogItem>> FilterAsync(string catalogBrand, string catalogType)
        {
            await Task.Delay(500);

            return MockCatalog
                .Where(c => c.CatalogBrand.Equals(catalogBrand, StringComparison.CurrentCultureIgnoreCase) &&
                c.CatalogType.Equals(catalogType, StringComparison.CurrentCultureIgnoreCase))
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
    }
}