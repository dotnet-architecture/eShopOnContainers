using eShopOnContainers.Core.Models.Location;
using eShopOnContainers.Core.Models.User;
using eShopOnContainers.Core.Services.Location;
using eShopOnContainers.Core.Services.Settings;
using eShopOnContainers.Core.ViewModels.Base;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using eShopOnContainers.Core.Services.Dependency;

namespace eShopOnContainers.Core.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _titleUseAzureServices;
        private string _descriptionUseAzureServices;
        private bool _useAzureServices;
        private string _titleUseFakeLocation;
        private string _descriptionUseFakeLocation;
        private bool _allowGpsLocation;
        private string _titleAllowGpsLocation;
        private string _descriptionAllowGpsLocation;
        private bool _useFakeLocation;
        private string _endpoint;
        private double _latitude;
        private double _longitude;
        private string _gpsWarningMessage;

        private readonly ISettingsService _settingsService;
        private readonly ILocationService _locationService;
        private readonly IDependencyService _dependencyService;

        public SettingsViewModel(ISettingsService settingsService, ILocationService locationService, IDependencyService dependencyService)
        {
            _settingsService = settingsService;
            _locationService = locationService;
            _dependencyService = dependencyService;

            _useAzureServices = !_settingsService.UseMocks;
            _endpoint = _settingsService.UrlBase;
            _latitude = double.Parse(_settingsService.Latitude, CultureInfo.CurrentCulture);
            _longitude = double.Parse(_settingsService.Longitude, CultureInfo.CurrentCulture);
            _useFakeLocation = _settingsService.UseFakeLocation;
            _allowGpsLocation = _settingsService.AllowGpsLocation;
            _gpsWarningMessage = string.Empty;
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

        public string TitleAllowGpsLocation
        {
            get => _titleAllowGpsLocation;
            set
            {
                _titleAllowGpsLocation = value;
                RaisePropertyChanged(() => TitleAllowGpsLocation);
            }
        }

        public string DescriptionAllowGpsLocation
        {
            get => _descriptionAllowGpsLocation;
            set
            {
                _descriptionAllowGpsLocation = value;
                RaisePropertyChanged(() => DescriptionAllowGpsLocation);
            }
        }

        public string GpsWarningMessage
        {
            get => _gpsWarningMessage;
            set
            {
                _gpsWarningMessage = value;
                RaisePropertyChanged(() => GpsWarningMessage);
            }
        }

        public string Endpoint
        {
            get => _endpoint;
            set
            {
                _endpoint = value;

                if (!string.IsNullOrEmpty(_endpoint))
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

        public bool AllowGpsLocation
        {
            get => _allowGpsLocation;
            set
            {
                _allowGpsLocation = value;

                UpdateAllowGpsLocation();

                RaisePropertyChanged(() => AllowGpsLocation);
            }
        }

        public bool UserIsLogged => !string.IsNullOrEmpty(_settingsService.AuthAccessToken);

        public ICommand ToggleMockServicesCommand => new Command(async () => await ToggleMockServicesAsync());

        public ICommand ToggleFakeLocationCommand => new Command(ToggleFakeLocationAsync);

        public ICommand ToggleSendLocationCommand => new Command(async () => await ToggleSendLocationAsync());

        public ICommand ToggleAllowGpsLocationCommand => new Command(ToggleAllowGpsLocation);

        public override Task InitializeAsync(object navigationData)
        {
            UpdateInfoUseAzureServices();
            UpdateInfoFakeLocation();
            UpdateInfoAllowGpsLocation();

            return base.InitializeAsync(navigationData);
        }

        private async Task ToggleMockServicesAsync()
        {
            ViewModelLocator.UpdateDependencies(!UseAzureServices);
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
                        _settingsService.AuthAccessToken = string.Empty;
                        _settingsService.AuthIdToken = string.Empty;

                        await NavigationService.NavigateToAsync<LoginViewModel>(new LogoutParameter { Logout = true });
                        await NavigationService.RemoveBackStackAsync();
                    }
                }
            }
        }

        private void ToggleFakeLocationAsync()
        {
            ViewModelLocator.UpdateDependencies(!UseAzureServices);
            UpdateInfoFakeLocation();
        }

        private async Task ToggleSendLocationAsync()
        {
            if (!_settingsService.UseMocks)
            {
                var locationRequest = new Location
                {
                    Latitude = _latitude,
                    Longitude = _longitude
                };
                var authToken = _settingsService.AuthAccessToken;

                await _locationService.UpdateUserLocation(locationRequest, authToken);
            }
        }

        private void ToggleAllowGpsLocation()
        {
            UpdateInfoAllowGpsLocation();
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
                TitleUseFakeLocation = "Use Real Location";
                DescriptionUseFakeLocation = "When enabling location, the app will attempt to use the location from the device.";

            }
            else
            {
                TitleUseFakeLocation = "Use Fake Location";
                DescriptionUseFakeLocation = "Fake Location data is added for marketing campaign testing.";
            }
        }

        private void UpdateInfoAllowGpsLocation()
        {
            if (!AllowGpsLocation)
            {
                TitleAllowGpsLocation = "GPS Location Disabled";
                DescriptionAllowGpsLocation = "When disabling location, you won't receive location campaigns based upon your location.";
            }
            else
            {
                TitleAllowGpsLocation = "GPS Location Enabled";
                DescriptionAllowGpsLocation = "When enabling location, you'll receive location campaigns based upon your location.";

            }
        }
        
        private void UpdateUseAzureServices()
        {
            // Save use mocks services to local storage
            _settingsService.UseMocks = !_useAzureServices;
        }

        private void UpdateEndpoint()
        {
            // Update remote endpoint (save to local storage)
            GlobalSetting.Instance.BaseEndpoint = _settingsService.UrlBase = _endpoint;
        }

        private void UpdateFakeLocation()
        {
            _settingsService.UseFakeLocation = _useFakeLocation;
        }

        private void UpdateLatitude()
        {
            // Update fake latitude (save to local storage)
            _settingsService.Latitude = _latitude.ToString();
        }

        private void UpdateLongitude()
        {
            // Update fake longitude (save to local storage)
            _settingsService.Longitude = _longitude.ToString();
        }

        private void UpdateAllowGpsLocation()
        {
            if (_allowGpsLocation)
            {
                var locator = _dependencyService.Get<ILocationServiceImplementation>();
                if (!locator.IsGeolocationEnabled)
                {
                    _allowGpsLocation = false;
                    GpsWarningMessage = "Enable the GPS sensor on your device";
                }
                else
                {
                    _settingsService.AllowGpsLocation = _allowGpsLocation;
                    GpsWarningMessage = string.Empty;
                }
            }
            else
            {
                _settingsService.AllowGpsLocation = _allowGpsLocation;
            }
        }
    }
}