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
        private static readonly string AccessTokenDefault = string.Empty;

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
    }
}