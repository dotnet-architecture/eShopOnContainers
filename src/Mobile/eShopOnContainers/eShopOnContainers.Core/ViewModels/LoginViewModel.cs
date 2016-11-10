using eShopOnContainers.Core.Validations;
using eShopOnContainers.ViewModels.Base;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private ValidatableObject<string> _userName;
        private ValidatableObject<string> _password;
        private bool _isValid;

        public LoginViewModel()
        {
            _userName = new ValidatableObject<string>();
            _password = new ValidatableObject<string>();

            AddValidations();
        }

        public ValidatableObject<string> UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public ValidatableObject<string> Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
                RaisePropertyChanged(() => IsValid);
            }
        }

        public ICommand SignInCommand => new Command(SignInAsync);

        private async void SignInAsync()
        {
            IsBusy = true;
            IsValid = true;
            bool isValid = Validate();
            bool isAuthenticated = false;

            if (isValid)
            {
                try
                {
                    isAuthenticated = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SignIn] Error signing in: {ex}");
                }
            }
            else
            {
                IsValid = false;
            }

            if (isAuthenticated)
            {
                await NavigationService.NavigateToAsync<MainViewModel>();
                await NavigationService.RemoveLastFromBackStackAsync();
            }

            IsBusy = false;
        }

        private bool Validate()
        {
            bool isValidUser = _userName.Validate();
            bool isValidPassword = _password.Validate();

            return isValidUser && isValidPassword;
        }

        private void AddValidations()
        {
            _userName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Username should not be empty" });
            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Password should not be empty" });
        }
    }
}
