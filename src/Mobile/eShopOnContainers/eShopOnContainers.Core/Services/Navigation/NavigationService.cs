using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.ViewModels;
using eShopOnContainers.Core.Views;
using eShopOnContainers.Core.ViewModels.Base;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eShopOnContainers.Services
{
    public class NavigationService : INavigationService
    {
		protected Application CurrentApplication
        {
            get
            {
                return Application.Current;
            }
        }

        public Task InitializeAsync()
        {
            if(string.IsNullOrEmpty(Settings.AuthAccessToken))
                return NavigateToAsync<LoginViewModel>();
            else
                return NavigateToAsync<MainViewModel>();
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task NavigateToAsync(Type viewModelType)
        {
            return InternalNavigateToAsync(viewModelType, null);
        }

        public Task NavigateToAsync(Type viewModelType, object parameter)
        {
            return InternalNavigateToAsync(viewModelType, parameter);
        }

        public async Task NavigateBackAsync()
        {
            if (CurrentApplication.MainPage is CatalogView)
            {
                var mainPage = CurrentApplication.MainPage as CatalogView;
                await mainPage.Navigation.PopAsync();
            }
            else if (CurrentApplication.MainPage != null)
            {
                await CurrentApplication.MainPage.Navigation.PopAsync();
            }
        }

        public virtual Task RemoveLastFromBackStackAsync()
        {
            var mainPage = CurrentApplication.MainPage as CustomNavigationView;

            if (mainPage != null)
            {
                mainPage.Navigation.RemovePage(
                    mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2]);
            }

            return Task.FromResult(true);
        }

        public virtual Task RemoveBackStackAsync()
        {
            var mainPage = CurrentApplication.MainPage as CustomNavigationView;

            if (mainPage != null)
            {
                for (int i = 0; i < mainPage.Navigation.NavigationStack.Count - 1; i++)
                {
                    var page = mainPage.Navigation.NavigationStack[i];
                    mainPage.Navigation.RemovePage(page);
                }
            }

            return Task.FromResult(true);
        }

        protected virtual async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreatePage(viewModelType, parameter);

            if (page is LoginView)
            {
                CurrentApplication.MainPage = new CustomNavigationView(page);
            }
            else
            {
                var navigationPage = CurrentApplication.MainPage as CustomNavigationView;
                if (navigationPage != null)
                {
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    CurrentApplication.MainPage = new CustomNavigationView(page);
                }
            }

            await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);
        }

        protected Type GetPageTypeForViewModel(Type viewModelType)
        {
			var viewName = viewModelType.FullName.Replace("Model", string.Empty);
			var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
			var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
			var viewType = Type.GetType(viewAssemblyName);
			return viewType;
        }

        protected Page CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
            {
				throw new Exception($"Cannot locate page type for {viewModelType}");
            }

            Page page = Activator.CreateInstance(pageType) as Page;
            return page;
        }
    }
}