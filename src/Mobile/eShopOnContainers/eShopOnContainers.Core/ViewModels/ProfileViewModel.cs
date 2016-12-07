using eShopOnContainers.Core.Extensions;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.Models.User;
using eShopOnContainers.Core.Services.Order;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private ObservableCollection<Order> _orders;

        private IOrderService _orderService;

        public ProfileViewModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                RaisePropertyChanged(() => Orders);
            }
        }

        public ICommand LogoutCommand => new Command(LogoutAsync);

        public ICommand OrderDetailCommand => new Command<Order>(OrderDetail);

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            var orders = await _orderService.GetOrdersAsync();
            Orders = orders.ToObservableCollection();

            IsBusy = false;
        }

        private async void LogoutAsync()
        {
            IsBusy = true;

            await NavigationService.NavigateToAsync<LoginViewModel>(new LogoutParameter { Logout = true });
            await NavigationService.RemoveBackStackAsync();

            IsBusy = false;
        }

        private void OrderDetail(Order order)
        {
            NavigationService.NavigateToAsync<OrderDetailViewModel>(order);
        }
    }
}