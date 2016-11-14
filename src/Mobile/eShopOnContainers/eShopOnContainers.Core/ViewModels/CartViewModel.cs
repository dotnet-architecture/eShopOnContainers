using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.Services.Orders;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.ViewModels.Base;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    public class CartViewModel : ViewModelBase
    {
        private int _badgeCount;
        private Order _order;

        private IOrdersService _orderService;

        public CartViewModel(IOrdersService orderService)
        {
            _orderService = orderService;
        }

        public int BadgeCount
        {
            get { return _badgeCount; }
            set
            {
                _badgeCount = value;
                RaisePropertyChanged(() => BadgeCount);
            }
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
            MessagingCenter.Subscribe<CatalogViewModel>(this, MessengerKeys.AddProduct, (sender) =>
            {
                BadgeCount++;
            });

            Order = await _orderService.GetCartAsync();
        }
    }
}
