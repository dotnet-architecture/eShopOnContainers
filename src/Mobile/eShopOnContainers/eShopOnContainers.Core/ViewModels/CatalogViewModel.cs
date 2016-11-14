using System.Threading.Tasks;
using eShopOnContainers.ViewModels.Base;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.Catalog;

namespace eShopOnContainers.Core.ViewModels
{
    public class CatalogViewModel : ViewModelBase
    {
        private ObservableCollection<CatalogItem> _products;
        private CatalogItem _product;

        private ICatalogService _productsService;

        public CatalogViewModel(ICatalogService productsService)
        {
            _productsService = productsService;
        }

        public ObservableCollection<CatalogItem> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                RaisePropertyChanged(() => Products);
            }
        }

        public CatalogItem Product
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
