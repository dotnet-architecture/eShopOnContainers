using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.Services.Identity;
using eShopOnContainers.Core.Services.OpenUrl;
using eShopOnContainers.Core.Validations;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.ViewModels.Base;
using IdentityModel.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private ValidatableObject<string> _userName;
        private ValidatableObject<string> _password;
        private bool _isMock;
        private bool _isValid;
        private string _authUrl;

        private IOpenUrlService _openUrlService;
        private IIdentityService _identityService;

        public LoginViewModel(IOpenUrlService openUrlService,
            IIdentityService identityService)
        {
            _openUrlService = openUrlService;
            _identityService = identityService;

            _userName = new ValidatableObject<string>();
            _password = new ValidatableObject<string>();

            IsMock = ViewModelLocator.Instance.UseMockService;

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

        public bool IsMock
        {
            get
            {
                return _isMock;
            }
            set
            {
                _isMock = value;
                RaisePropertyChanged(() => IsMock);
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

        public string LoginUrl
        {
            get
            {
                return _authUrl;
            }
            set
            {
                _authUrl = value;
                RaisePropertyChanged(() => LoginUrl);
            }
        }

        public ICommand MockSignInCommand => new Command(MockSignInAsync);

        public ICommand SignInCommand => new Command(async () => await SignInAsync());

        public ICommand RegisterCommand => new Command(Register);

        public ICommand NavigateCommand => new Command<string>(NavigateAsync);

        public override Task InitializeAsync(object navigationData)
        {
            MessagingCenter.Subscribe<ProfileViewModel>(this, MessengerKeys.Logout, (sender) =>
            {
                Logout();
            });

            return base.InitializeAsync(navigationData);
        }

        private async void MockSignInAsync()
        {
            IsBusy = true;
            IsValid = true;
            bool isValid = Validate();
            bool isAuthenticated = false;

            if (isValid)
            {
                try
                {
                    await Task.Delay(1000);

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

        private async Task SignInAsync()
        {
            IsBusy = true;

            await Task.Delay(500);

            LoginUrl = _identityService.CreateAuthorizeRequest();

            IsValid = true;

            IsBusy = false;
        }

        private void Register()
        {
            _openUrlService.OpenUrl(GlobalSetting.RegisterWebsite);
        }

        private void Logout()
        {
            var token = Settings.AuthAccessToken;
            var logoutRequest = _identityService.CreateLogoutRequest(token);

            if(string.IsNullOrEmpty(logoutRequest))
            {
                IsValid = false;
                LoginUrl = logoutRequest;
                Settings.AuthAccessToken = string.Empty;
            }
        }

        private async void NavigateAsync(string url)
        {
            if (url.Contains(GlobalSetting.IdentityCallback))
            {
                // Parse response
                var authResponse = new AuthorizeResponse(url);

                if (!string.IsNullOrWhiteSpace(authResponse.AccessToken))
                {
                    string decodedTokens = _identityService.DecodeToken(authResponse.AccessToken);
                    
                    if(decodedTokens != null)
                    {
                        Settings.AuthAccessToken = decodedTokens;

                        await NavigationService.NavigateToAsync<MainViewModel>();
                        await NavigationService.RemoveLastFromBackStackAsync();
                    }
                }
            }
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