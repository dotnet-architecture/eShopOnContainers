using eShopOnContainers.Core.Models.Products;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Services.Products
{
    public class FakeProductsService : IProductsService
    {
        public async Task<ObservableCollection<Product>> GetProductsAsync()
        {
            await Task.Delay(500);

            return new ObservableCollection<Product>
            {
                new Product { Image = Device.OS != TargetPlatform.Windows ? "fake_product_01" : "Assets/fake_product_01.png", Name = ".NET Bot Blue Sweatshirt (M)", Price = 19.50 },
                new Product { Image = Device.OS != TargetPlatform.Windows ? "fake_product_02": "Assets/fake_product_02.png", Name = ".NET Bot Purple Sweatshirt (M)", Price = 19.50 },
                new Product { Image = Device.OS != TargetPlatform.Windows ? "fake_product_03": "Assets/fake_product_03.png", Name = ".NET Bot Black Sweatshirt (M)", Price = 19.95 }
            };
        }
    }
}
