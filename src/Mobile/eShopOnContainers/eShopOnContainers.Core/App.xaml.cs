using System;
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

            if (!Settings.UseFakeLocation)
            {
                await GetRealLocation();
            }

            if (!Settings.UseMocks && !string.IsNullOrEmpty(Settings.UserId))
            {
                await SendCurrentLocation();
            }

            base.OnResume();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        private async Task GetRealLocation()
        {
            var locator = CrossGeolocator.Current;
            locator.AllowsBackgroundUpdates = true;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(20000);

            Settings.Latitude = position.Latitude;
            Settings.Longitude = position.Longitude;
        }

        private async Task SendCurrentLocation()
        {
            var location = new Location
            {
                Latitude = Settings.Latitude,
                Longitude = Settings.Longitude
            };

            var locationService = ViewModelLocator.Resolve<ILocationService>();
            await locationService.UpdateUserLocation(location,
                Settings.AuthAccessToken);
        }
    }
}