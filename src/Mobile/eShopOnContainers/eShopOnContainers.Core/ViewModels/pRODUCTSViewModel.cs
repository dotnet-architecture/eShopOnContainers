using System.Threading.Tasks;
using eShopOnContainers.ViewModels.Base;
using eShopOnContainers.Core.Services.Products;
using System.Collections.ObjectModel;
using eShopOnContainers.Core.Models.Products;
using Xamarin.Forms;
using eShopOnContainers.Core.ViewModels.Base;

namespace eShopOnContainers.Core.ViewModels
{
    public class ProductsViewModel : ViewModelBase
    {
        private ObservableCollection<Product> _products;
        private Product _product;

        private IProductsService _productsService;

        public ProductsViewModel(IProductsService productsService)
        {
            _productsService = productsService;
        }

        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                RaisePropertyChanged(() => Products);
            }
        }

        public Product Product
        {
            get { return _product; }
            set
            {
                _product = value;

                if (_product != null)
                {
                    AddProduct();
                }
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            Products = await _productsService.GetProductsAsync();
        }

        private void AddProduct()
        {
            MessagingCenter.Send(this, MessengerKeys.AddProduct);
        }
    }
}
