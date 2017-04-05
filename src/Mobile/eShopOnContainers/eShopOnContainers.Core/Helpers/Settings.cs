using eShopOnContainers.Core.ViewModels.Base;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace eShopOnContainers.Core.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string AccessToken = "access_token";
        private const string IdToken = "id_token";
        private const string IdUseMocks = "use_mocks";
        private const string IdUrlBase = "url_base";
        private static readonly string AccessTokenDefault = string.Empty;
        private static readonly string IdTokenDefault = string.Empty;
		private static readonly bool UseMocksDefault = true;
        private static readonly string UrlBaseDefault = GlobalSetting.Instance.BaseEndpoint;

        #endregion


        public static string AuthAccessToken
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(AccessToken, AccessTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(AccessToken, value);
            }
        }

        public static string AuthIdToken
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(IdToken, IdTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(IdToken, value);
            }
        }


        public static bool UseMocks
        {
            get
            {
                return AppSettings.GetValueOrDefault<bool>(IdUseMocks, UseMocksDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<bool>(IdUseMocks, value);
            }
        }

        public static string UrlBase
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(IdUrlBase, UrlBaseDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(IdUrlBase, value);
            }
        }
    }
}