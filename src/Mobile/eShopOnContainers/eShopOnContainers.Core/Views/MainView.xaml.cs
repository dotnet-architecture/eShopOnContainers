using eShopOnContainers.Core.ViewModels;
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

            var homeViewModel = ViewModelLocator.Instance.Resolve<CatalogViewModel>();
            await homeViewModel.InitializeAsync(null);
            HomeView.BindingContext = homeViewModel;

            var profileViewModel = ViewModelLocator.Instance.Resolve<ProfileViewModel>();
            await profileViewModel.InitializeAsync(null);
            ProfileView.BindingContext = profileViewModel;

            var cartViewModel = ViewModelLocator.Instance.Resolve<CartViewModel>();
            await cartViewModel.InitializeAsync(null);
            CartView.BindingContext = cartViewModel;
        }
    }
}
