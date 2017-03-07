using eShopOnContainers.Core.ViewModels;
using eShopOnContainers.Core.ViewModels.Base;
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

			await ((CatalogViewModel)HomeView.BindingContext).InitializeAsync(null);
			await ((BasketViewModel)BasketView.BindingContext).InitializeAsync(null);
			await ((ProfileViewModel)ProfileView.BindingContext).InitializeAsync(null);
        }
    }
}
