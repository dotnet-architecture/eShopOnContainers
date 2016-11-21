using eShopOnContainers.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private bool _useMockServices;

        public SettingsViewModel()
        {
            UseMockServices = ViewModelLocator.Instance.UseMockService;
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
        }
    }
}