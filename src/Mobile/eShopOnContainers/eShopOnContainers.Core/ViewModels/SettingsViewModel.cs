using eShopOnContainers.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _title;
        private string _description;
        private bool _useMockServices;

        public SettingsViewModel()
        {
            UseMockServices = ViewModelLocator.Instance.UseMockService;
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

        public bool UseMockServices
        {
            get { return _useMockServices; }
            set
            {
                _useMockServices = value;
                RaisePropertyChanged(() => UseMockServices);
            }
        }

        public ICommand MockServicesCommand => new Command(MockServices);

        private void MockServices()
        {
            ViewModelLocator.Instance.UpdateServices(UseMockServices);
            UpdateInfo();
        }

        public override Task InitializeAsync(object navigationData)
        {
            UpdateInfo();

            return base.InitializeAsync(navigationData);
        }

        private void UpdateInfo()
        {
            if (!UseMockServices)
            {
                Title = "Use Mock Services";
                Description = "Mock Services are simulated objects that mimic the behavior of real services in controlled ways";
            }
            else
            {
                Title = "Use Azure Services";
                Description = "Azure Services are real objects that required a valid internet connection";
            }
        }
    }
}