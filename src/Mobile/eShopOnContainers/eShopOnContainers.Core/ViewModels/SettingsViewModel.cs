using eShopOnContainers.Core.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.Models.User;
using System;
using eShopOnContainers.Core.Models.Location;
using eShopOnContainers.Core.Services.Location;

namespace eShopOnContainers.Core.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _titleUseAzureServices;
        private string _descriptionUseAzureServices;
        private bool _useAzureServices;
        private string _titleUseFakeLocation;
        private string _descriptionUseFakeLocation;
        private bool _useFakeLocation;
        private string _endpoint;
        private double _latitude;
        private double _longitude;
        private ILocationService _locationService;

        public SettingsViewModel(ILocationService locationService)
        {
            UseAzureServices = !Settings.UseMocks;
            _useFakeLocation = Settings.UseFakeLocation;
            _latitude = Settings.FakeLatitude;
            _longitude = Settings.FakeLongitude;
            _locationService = locationService;
        }

        public string TitleUseAzureServices
        {
            get { return _titleUseAzureServices; }
            set
            {
                _titleUseAzureServices = value;
                RaisePropertyChanged(() => TitleUseAzureServices);
            }
        }

        public string DescriptionUseAzureServices
        {
            get { return _descriptionUseAzureServices; }
            set
            {
                _descriptionUseAzureServices = value;
                RaisePropertyChanged(() => DescriptionUseAzureServices);
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

        public string TitleUseFakeLocation
        {
            get { return _titleUseFakeLocation; }
            set
            {
                _titleUseFakeLocation = value;
                RaisePropertyChanged(() => TitleUseFakeLocation);
            }
        }

        public string DescriptionUseFakeLocation
        {
            get { return _descriptionUseFakeLocation; }
            set
            {
                _descriptionUseFakeLocation = value;
                RaisePropertyChanged(() => DescriptionUseFakeLocation);
            }
        }

        public bool UseFakeLocation
        {
            get { return _useFakeLocation; }
            set
            {
                _useFakeLocation = value;

                // Save use fake location services to local storage
                Settings.UseFakeLocation = _useFakeLocation;
                RaisePropertyChanged(() => UseFakeLocation);
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

        public double Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;

                UpdateLatitude(_latitude);

                RaisePropertyChanged(() => Latitude);
            }
        }

        public double Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;

                UpdateLongitude(_longitude);

                RaisePropertyChanged(() => Longitude);
            }
        }

        public ICommand ToggleMockServicesCommand => new Command(async () => await ToggleMockServicesAsync());

        public ICommand ToggleFakeLocationCommand => new Command(() => ToggleFakeLocationAsync());

        public ICommand ToggleSendLocationCommand => new Command(async () => await ToggleSendLocationAsync());

        public override Task InitializeAsync(object navigationData)
        {
            UpdateInfo();
            UpdateInfoFakeLocation();
            
            Endpoint = Settings.UrlBase;
            _latitude = Settings.FakeLatitude;
            _longitude = Settings.FakeLongitude;
            _useFakeLocation = Settings.UseFakeLocation;
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

        private void ToggleFakeLocationAsync()
        {
            ViewModelLocator.RegisterDependencies(!UseAzureServices);
            UpdateInfoFakeLocation();
        }

        private async Task ToggleSendLocationAsync()
        {
            LocationRequest locationRequest = new LocationRequest
            {
                Latitude = _latitude,
                Longitude = _longitude
            };

            await _locationService.UpdateUserLocation(locationRequest);
        }

        private void UpdateInfo()
        {
            if (!UseAzureServices)
            {
                TitleUseAzureServices = "Use Mock Services";
                DescriptionUseAzureServices = "Mock Services are simulated objects that mimic the behavior of real services using a controlled approach.";
            }
            else
            {
                TitleUseAzureServices = "Use Microservices/Containers from eShopOnContainers";
                DescriptionUseAzureServices = "When enabling the use of microservices/containers, the app will attempt to use real services deployed as Docker containers at the specified base endpoint, which will must be reachable through the network.";
            }
        }

        private void UpdateInfoFakeLocation()
        {
            if (!UseFakeLocation)
            {
                TitleUseFakeLocation = "Use Fake Location";
                DescriptionUseFakeLocation = "Fake Location are added for marketing campaign testing.";
            }
            else
            {
                TitleUseFakeLocation = "Use Real Location";
                DescriptionUseFakeLocation = "When enabling the use of real location, the app will attempt to use real location from the device.";
            }
        }

        private void UpdateEndpoint(string endpoint)
        {
            // Update remote endpoint (save to local storage)
            Settings.UrlBase = endpoint;
        }

        private void UpdateLatitude(double latitude)
        {
            // Update fake latitude (save to local storage)
            Settings.FakeLatitude = latitude;
        }

        private void UpdateLongitude(double longitude)
        {
            // Update fake longitude (save to local storage)
            Settings.FakeLongitude = longitude;
        }
    }
}