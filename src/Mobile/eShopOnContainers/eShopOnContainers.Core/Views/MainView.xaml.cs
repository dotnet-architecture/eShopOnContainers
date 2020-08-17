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

            MessagingCenter.Subscribe<MainViewModel, int>(this, MessageKeys.ChangeTab, (sender, arg) =>
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
                    case 3:
                        CurrentPage = CampaignView;
                        break;
                }
            });

			await ((CatalogViewModel)HomeView.BindingContext).InitializeAsync(null);
			await ((BasketViewModel)BasketView.BindingContext).InitializeAsync(null);
			await ((ProfileViewModel)ProfileView.BindingContext).InitializeAsync(null);
            await ((CampaignViewModel)CampaignView.BindingContext).InitializeAsync(null);
        }

        protected override async void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            if (CurrentPage is BasketView)
            {
                // Force basket view refresh every time we access it
                await (BasketView.BindingContext as ViewModelBase).InitializeAsync(null);
            }
            else if (CurrentPage is CampaignView)
            {
                // Force campaign view refresh every time we access it
                await (CampaignView.BindingContext as ViewModelBase).InitializeAsync(null);
            }
            else if (CurrentPage is ProfileView)
            {
                // Force profile view refresh every time we access it
                await (ProfileView.BindingContext as ViewModelBase).InitializeAsync(null);
            }
        }
    }
}
