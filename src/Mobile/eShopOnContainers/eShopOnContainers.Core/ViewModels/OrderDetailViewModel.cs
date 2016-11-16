using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.ViewModels.Base;
using eShopOnContainers.Core.Services.Orders;
using eShopOnContainers.Core.Services.Catalog;
using eShopOnContainers.Core.Services.User;
using eShopOnContainers.Core.Models.User;

namespace eShopOnContainers.Core.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase
    {
        private Order _order;
        private User _user;

        private IOrdersService _orderService;
        private ICatalogService _catalogService;
        private IUserService _userService;

        public OrderDetailViewModel(IOrdersService orderService,
            ICatalogService catalogService,
            IUserService userService)
        {
            _orderService = orderService;
            _catalogService = catalogService;
            _userService = userService;
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

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
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
                User = await _userService.GetUserAsync();

                IsBusy = false;
            }
        }
    }
}