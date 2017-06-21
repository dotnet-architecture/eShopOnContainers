namespace eShopOnContainers.Core.ViewModels
{
    using System.Windows.Input;
    using Xamarin.Forms;
    using System.Threading.Tasks;
    using Helpers;
    using Models.User;
    using Base;
    using Models.Location;
    using Services.Location;

    public class SettingsViewModel : ViewModelBase
    {
        private string _titleUseAzureServices;
        private string _descriptionUseAzureServices;
        private bool _useAzureServices;
        private string _titleUseFakeLocation;
        private string _descriptionUseFakeLocation;
        private bool _gpsUsage;
        private string _titleGpsUsage;
        private string _descriptionGpsUsage;
        private bool _useFakeLocation;
        private string _endpoint;
        private double _latitude;
        private double _longitude;
        
        private readonly ILocationService _locationService;

        public SettingsViewModel(ILocationService locationService)
        {
            _locationService = locationService;

            _useAzureServices = !Settings.UseMocks;
            _endpoint = Settings.UrlBase;
            _latitude = Settings.Latitude;
            _longitude = Settings.Longitude;
            _useFakeLocation = Settings.UseFakeLocation;
            _gpsUsage = Settings.GpsUsage;
        }

        public string TitleUseAzureServices
        {
            get => _titleUseAzureServices;
            set
            {
                _titleUseAzureServices = value;
                RaisePropertyChanged(() => TitleUseAzureServices);
            }
        }

        public string DescriptionUseAzureServices
        {
            get => _descriptionUseAzureServices;
            set
            {
                _descriptionUseAzureServices = value;
                RaisePropertyChanged(() => DescriptionUseAzureServices);
            }
        }

        public bool UseAzureServices
        {
            get => _useAzureServices;
            set
            {
                _useAzureServices = value;

                UpdateUseAzureServices();
                
                RaisePropertyChanged(() => UseAzureServices);
            }
        }

        public string TitleUseFakeLocation
        {
            get => _titleUseFakeLocation;
            set
            {
                _titleUseFakeLocation = value;
                RaisePropertyChanged(() => TitleUseFakeLocation);
            }
        }

        public string DescriptionUseFakeLocation
        {
            get => _descriptionUseFakeLocation;
            set
            {
                _descriptionUseFakeLocation = value;
                RaisePropertyChanged(() => DescriptionUseFakeLocation);
            }
        }

        public bool UseFakeLocation
        {
            get => _useFakeLocation;
            set
            {
                _useFakeLocation = value;

                UpdateFakeLocation();

                RaisePropertyChanged(() => UseFakeLocation);
            }
        }

        public string TitleGpsUsage
        {
            get => _titleGpsUsage;
            set
            {
                _titleGpsUsage = value;
                RaisePropertyChanged(() => TitleGpsUsage);
            }
        }

        public string DescriptionGpsUsage
        {
            get => _descriptionGpsUsage;
            set
            {
                _descriptionGpsUsage = value;
                RaisePropertyChanged(() => DescriptionGpsUsage);
            }
        }

        public string Endpoint
        {
            get => _endpoint;
            set
            {
                _endpoint = value;

                if(!string.IsNullOrEmpty(_endpoint))
                {
                    UpdateEndpoint();
                }

                RaisePropertyChanged(() => Endpoint);
            }
        }

        public double Latitude
        {
            get => _latitude;
            set
            {
                _latitude = value;

                UpdateLatitude();

                RaisePropertyChanged(() => Latitude);
            }
        }

        public double Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;

                UpdateLongitude();

                RaisePropertyChanged(() => Longitude);
            }
        }

        public bool GpsUsage
        {
            get => _gpsUsage;
            set
            {
                _gpsUsage = value;

                UpdateGpsUsage();

                RaisePropertyChanged(() => GpsUsage);
            }
        }

        public bool UserIsLogged => !string.IsNullOrEmpty(Settings.AuthAccessToken);

        public ICommand ToggleMockServicesCommand => new Command(async () => await ToggleMockServicesAsync());

        public ICommand ToggleFakeLocationCommand => new Command(ToggleFakeLocationAsync);

        public ICommand ToggleSendLocationCommand => new Command(async () => await ToggleSendLocationAsync());

        public ICommand ToggleGpsUsageCommand => new Command(ToggleGpsUsage);

        public override Task InitializeAsync(object navigationData)
        {
            UpdateInfoUseAzureServices();
            UpdateInfoFakeLocation();
            UpdateInfoGpsUsage();

            return base.InitializeAsync(navigationData);
        }

		private async Task ToggleMockServicesAsync()
		{
			ViewModelLocator.RegisterDependencies(!UseAzureServices);
			UpdateInfoUseAzureServices();

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
            if (!Settings.UseMocks)
            {
                var locationRequest = new Location
                {
                    Latitude = _latitude,
                    Longitude = _longitude
                };
                var authToken = Settings.AuthAccessToken;

                await _locationService.UpdateUserLocation(locationRequest, authToken);
            } 
        }

        private void ToggleGpsUsage()
        {
            UpdateInfoGpsUsage();
        }

        private void UpdateInfoUseAzureServices()
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

        private void UpdateInfoGpsUsage()
        {
            if (!GpsUsage)
            {
                TitleGpsUsage = "Enable GPS";
                DescriptionGpsUsage = "When enabling the use of device gps you will get the location campaigns through your real location.";
            }
            else
            {
                TitleGpsUsage = "Disable GPS";
                DescriptionGpsUsage = "When disabling the use of device gps you won't get the location campaigns through your real location.";
            }
        }


        private void UpdateUseAzureServices()
        {
            // Save use mocks services to local storage
            Settings.UseMocks = !_useAzureServices;
        }

        private void UpdateEndpoint()
        {
            // Update remote endpoint (save to local storage)
            GlobalSetting.Instance.BaseEndpoint = Settings.UrlBase = _endpoint;
        }

        private void UpdateFakeLocation()
        {
            Settings.UseFakeLocation = _useFakeLocation;
        }

        private void UpdateLatitude()
        {
            // Update fake latitude (save to local storage)
            Settings.Latitude = _latitude;
        }

        private void UpdateLongitude()
        {
            // Update fake longitude (save to local storage)
            Settings.Longitude = _longitude;
        }

        private void UpdateGpsUsage()
        {
            Settings.GpsUsage = _gpsUsage;
        }
    }
}