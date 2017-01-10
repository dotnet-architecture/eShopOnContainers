using eShopOnContainers.Core.ViewModels;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.ViewModels.Base;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Views
{
    public partial class MainView : TabbedPage
    {
        public MainView()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<MainViewModel, int>(this, MessengerKeys.ChangeTab, (sender, arg) =>
            {
               switch(arg)
                {
                    case 0:
                        CurrentPage = HomeView;
                        break;
                    case 1:
                        CurrentPage = ProfileView;
                        break;
                    case 2:
                        CurrentPage = BasketView;
                        break;
                }
            });

            var homeViewModel = ViewModelLocator.Instance.Resolve<CatalogViewModel>();
            await homeViewModel.InitializeAsync(null);
            HomeView.BindingContext = homeViewModel;

            var basketViewModel = ViewModelLocator.Instance.Resolve<BasketViewModel>();
            await basketViewModel.InitializeAsync(null);
            BasketView.BindingContext = basketViewModel;

            var profileViewModel = ViewModelLocator.Instance.Resolve<ProfileViewModel>();
            await profileViewModel.InitializeAsync(null);
            ProfileView.BindingContext = profileViewModel;
        }
    }
}
