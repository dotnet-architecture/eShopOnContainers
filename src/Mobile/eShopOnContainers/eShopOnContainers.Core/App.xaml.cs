using System;
using System.Globalization;
using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Services;
using eShopOnContainers.Core.ViewModels.Base;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Location;
using eShopOnContainers.Core.Services.Location;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace eShopOnContainers
{
    public partial class App : Application
    {
        public bool UseMockServices { get; set; }

        public App()
        {
            InitializeComponent();

            InitApp();

			if (Device.RuntimePlatform == Device.Windows)
            {
                InitNavigation();
            }
        }

        private void InitApp()
        {
            UseMockServices = Settings.UseMocks;
            ViewModelLocator.RegisterDependencies(UseMockServices);
        }

        private Task InitNavigation()
        {
            var navigationService = ViewModelLocator.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }


        protected override async void OnStart()
        {
            base.OnStart();

			if (Device.RuntimePlatform != Device.Windows)
            {
                await InitNavigation();
            }

            if (Settings.AllowGpsLocation && !Settings.UseFakeLocation)
            {
                await GetGpsLocation();
            }

            if (!Settings.UseMocks && !string.IsNullOrEmpty(Settings.AuthAccessToken))
            {
                await SendCurrentLocation();
            }

            base.OnResume();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        private async Task GetGpsLocation()
        {
            var locator = CrossGeolocator.Current;

            if (locator.IsGeolocationEnabled && locator.IsGeolocationAvailable)
            { 
                locator.AllowsBackgroundUpdates = true;
                locator.DesiredAccuracy = 50;

                var position = await locator.GetPositionAsync();

                Settings.Latitude = position.Latitude.ToString();
                Settings.Longitude = position.Longitude.ToString();
            }
            else
            {
                Settings.AllowGpsLocation = false;
            }
        }

        private async Task SendCurrentLocation()
        {
            var location = new Location
            {
                Latitude = double.Parse(Settings.Latitude, CultureInfo.InvariantCulture),
                Longitude = double.Parse(Settings.Longitude, CultureInfo.InvariantCulture)
            };

            var locationService = ViewModelLocator.Resolve<ILocationService>();
            await locationService.UpdateUserLocation(location,
                Settings.AuthAccessToken);
        }
    }
}