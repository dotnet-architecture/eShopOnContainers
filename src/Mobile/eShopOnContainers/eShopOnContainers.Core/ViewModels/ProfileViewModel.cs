using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.Services.Orders;
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

        private IOrdersService _ordersService;

        public ProfileViewModel(IOrdersService ordersService)
        {
            _ordersService = ordersService;
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

        public override async Task InitializeAsync(object navigationData)
        {
            Orders = await _ordersService.GetOrdersAsync();
        }

        private async void LogoutAsync()
        {
            await NavigationService.NavigateToAsync<LoginViewModel>();
            await NavigationService.RemoveBackStackAsync();
        }
    }
}