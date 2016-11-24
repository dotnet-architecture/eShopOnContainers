using eShopOnContainers.Core.Extensions;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.Services.User;
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

        private IUserService _userService;

        public ProfileViewModel(IUserService userService)
        {
            _userService = userService;
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

            var orders = await _userService.GetOrdersAsync();
            Orders = orders.ToObservableCollection();

            IsBusy = false;
        }

        private async void LogoutAsync()
        {
            IsBusy = true;

            await NavigationService.NavigateToAsync<LoginViewModel>();
            await NavigationService.RemoveBackStackAsync();

            IsBusy = false;
        }

        private void OrderDetail(Order order)
        {
            NavigationService.NavigateToAsync<OrderDetailViewModel>(order);
        }
    }
}