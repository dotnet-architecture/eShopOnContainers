using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Services;
using eShopOnContainers.Core.ViewModels.Base;
using System.Threading.Tasks;
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

            if (Device.OS == TargetPlatform.Windows)
            {
                InitNavigation();
            }
        }

        private void InitApp()
        {
            UseMockServices = Settings.UseMocks;
			ViewModelLocator.Initialize();
            ViewModelLocator.UpdateDependencies(UseMockServices);
        }

        private Task InitNavigation()
        {
            var navigationService = ViewModelLocator.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            if (Device.OS != TargetPlatform.Windows)
            {
                await InitNavigation();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
