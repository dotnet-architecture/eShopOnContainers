using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.ViewModels.Base;
using eShopOnContainers.Core.Services.Orders;
using eShopOnContainers.Core.Services.Catalog;

namespace eShopOnContainers.Core.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase
    {
        private Order _order;

        private IOrdersService _orderService;
        private ICatalogService _catalogService;

        public OrderDetailViewModel(IOrdersService orderService,
            ICatalogService catalogService)
        {
            _orderService = orderService;
            _catalogService = catalogService;
        }

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
                RaisePropertyChanged(() => Order);
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if(navigationData is Order)
            {
                IsBusy = true;

                var order = navigationData as Order;
                   
                foreach (var orderItem in order.OrderItems)
                {
                    var catalogItem = await _catalogService.GetCatalogItemAsync(orderItem.ProductId.ToString());
                    orderItem.ProductImage = catalogItem.PictureUri;
                }

                Order = order;

                IsBusy = false;
            }
        }
    }
}