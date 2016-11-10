using eShopOnContainers.ViewModels.Base;
using System;
using System.Threading.Tasks;

namespace eShopOnContainers.Services
{
    public interface INavigationService
    {
        Task InitializeAsync();

        Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        Task NavigateToAsync(Type viewModelType);

        Task NavigateToAsync(Type viewModelType, object parameter);

        Task NavigateBackAsync();

        Task RemoveLastFromBackStackAsync();

        Task RemoveBackStackAsync();
    }
}