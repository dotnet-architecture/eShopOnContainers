using eShopOnContainers.Core.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.Models.User;

namespace eShopOnContainers.Core.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _title;
        private string _description;
        private bool _useAzureServices;
        private string _endpoint;

        public SettingsViewModel()
        {
            UseAzureServices = !Settings.UseMocks;
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        public bool UseAzureServices
        {
            get { return _useAzureServices; }
            set
            {
                _useAzureServices = value;

                // Save use mocks services to local storage
                Settings.UseMocks = !_useAzureServices;
                RaisePropertyChanged(() => UseAzureServices);
            }
        }

        public string Endpoint
        {
            get { return _endpoint; }
            set
            {
                _endpoint = value;

                if(!string.IsNullOrEmpty(_endpoint))
                {
                    UpdateEndpoint(_endpoint);
                }

                RaisePropertyChanged(() => Endpoint);
            }
        }

        public ICommand ToggleMockServicesCommand => new Command(async () => await ToggleMockServicesAsync());

        public override Task InitializeAsync(object navigationData)
        {
            UpdateInfo();
            Endpoint = Settings.UrlBase;
            return base.InitializeAsync(navigationData);
        }

		private async Task ToggleMockServicesAsync()
		{
			ViewModelLocator.RegisterDependencies(!UseAzureServices);
			UpdateInfo();

			var previousPageViewModel = NavigationService.PreviousPageViewModel;
			if (previousPageViewModel != null)
			{
				if (previousPageViewModel is MainViewModel)
				{
					// Slight delay so that page navigation isn't instantaneous
					await Task.Delay(1000);
					if (UseAzureServices)
					{
						Settings.AuthAccessToken = string.Empty;
						Settings.AuthIdToken = string.Empty;
						await NavigationService.NavigateToAsync<LoginViewModel>(new LogoutParameter { Logout = true });
						await NavigationService.RemoveBackStackAsync();
					}
				}
			}
		}

		private void UpdateInfo()
        {
            if (!UseAzureServices)
            {
                Title = "Use Mock Services";
                Description = "Mock Services are simulated objects that mimic the behavior of real services using a controlled approach.";
            }
            else
            {
                Title = "Use Microservices/Containers from eShopOnContainers";
                Description = "When enabling the use of microservices/containers, the app will attempt to use real services deployed as Docker containers at the specified base endpoint, which will must be reachable through the network.";
            }
        }

        private void UpdateEndpoint(string endpoint)
        {
            // Update remote endpoint (save to local storage)
            Settings.UrlBase = endpoint;
        }
    }
}