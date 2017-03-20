using eShopOnContainers.Core;
using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Services;
using System.Threading.Tasks;

namespace eShopOnContainers.ViewModels.Base
{
    public abstract class ViewModelBase : ExtendedBindableObject
    {
        protected readonly IDialogService DialogService;
        protected readonly INavigationService NavigationService;

        private bool _isBusy;

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public ViewModelBase()
        {
            DialogService = ViewModelLocator.Instance.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Instance.Resolve<INavigationService>();
            GlobalSetting.Instance.BaseEndpoint = Settings.UrlBase;
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }
    }
}