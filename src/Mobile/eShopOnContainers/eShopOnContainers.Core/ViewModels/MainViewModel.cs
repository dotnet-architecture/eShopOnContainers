﻿using System.Threading.Tasks;
using eShopOnContainers.ViewModels.Base;
using eShopOnContainers.Core.Models.Navigation;
using Xamarin.Forms;
using eShopOnContainers.Core.ViewModels.Base;
using System.Windows.Input;

namespace eShopOnContainers.Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand SettingsCommand => new Command(async () => await SettingsAsync());

        public override Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            if (navigationData is TabParameter)
            {
                // Change selected application tab
                var tabIndex = ((TabParameter)navigationData).TabIndex;
                MessagingCenter.Send(this, MessageKeys.ChangeTab, tabIndex);
            }

            return base.InitializeAsync(navigationData);
        }

        private async Task SettingsAsync()
        {
            await NavigationService.NavigateToAsync<SettingsViewModel>();
        }
    }
}