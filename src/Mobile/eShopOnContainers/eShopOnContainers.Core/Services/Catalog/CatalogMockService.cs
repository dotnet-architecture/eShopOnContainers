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
        private ObservableCollection<CatalogItem> MockCatalog = new ObservableCollection<CatalogItem>
        {
            new CatalogItem { Id = "1", PictureUri = Device.OS != TargetPlatform.Windows? "fake_product_01" : "Assets/fake_product_01.png", Name = ".NET Bot Blue Sweatshirt (M)", Price = 19.50M },
            new CatalogItem { Id = "2", PictureUri = Device.OS != TargetPlatform.Windows? "fake_product_02": "Assets/fake_product_02.png", Name = ".NET Bot Purple Sweatshirt (M)", Price = 19.50M },
            new CatalogItem { Id = "3", PictureUri = Device.OS != TargetPlatform.Windows? "fake_product_03": "Assets/fake_product_03.png", Name = ".NET Bot Black Sweatshirt (M)", Price = 19.95M }
        };

        public async Task<ObservableCollection<CatalogItem>> GetCatalogAsync()
        {
            await Task.Delay(500);

            return MockCatalog;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(string id)
        {
            await Task.Delay(500);

            return MockCatalog.FirstOrDefault(c => c.Id == id);
        }
    }
}