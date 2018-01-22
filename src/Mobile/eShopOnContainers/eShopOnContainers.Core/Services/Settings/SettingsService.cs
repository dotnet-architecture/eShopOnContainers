using eShopOnContainers.Core.Services.Dependency;

namespace eShopOnContainers.Core.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsServiceImplementation _settingsService;

        ISettingsServiceImplementation AppSettings
        {
            get { return _settingsService; }
        }

        public SettingsService(IDependencyService dependencyService)
        {
            _settingsService = dependencyService.Get<ISettingsServiceImplementation>();
        }

        #region Setting Constants

        private const string AccessToken = "access_token";
        private const string IdToken = "id_token";
        private const string IdUseMocks = "use_mocks";
        private const string IdUrlBase = "url_base";
        private const string IdUseFakeLocation = "use_fake_location";
        private const string IdLatitude = "latitude";
        private const string IdLongitude = "longitude";
        private const string IdAllowGpsLocation = "allow_gps_location";
        private readonly string AccessTokenDefault = string.Empty;
        private readonly string IdTokenDefault = string.Empty;
        private readonly bool UseMocksDefault = true;
        private readonly bool UseFakeLocationDefault = false;
        private readonly bool AllowGpsLocationDefault = false;
        private readonly double FakeLatitudeDefault = 47.604610d;
        private readonly double FakeLongitudeDefault = -122.315752d;
        private readonly string UrlBaseDefault = GlobalSetting.Instance.BaseEndpoint;

        #endregion

        public string AuthAccessToken
        {
            get => AppSettings.GetValueOrDefault(AccessToken, AccessTokenDefault);
            set => AppSettings.AddOrUpdateValue(AccessToken, value);
        }

        public string AuthIdToken
        {
            get => AppSettings.GetValueOrDefault(IdToken, IdTokenDefault);
            set => AppSettings.AddOrUpdateValue(IdToken, value);
        }

        public bool UseMocks
        {
            get => AppSettings.GetValueOrDefault(IdUseMocks, UseMocksDefault);
            set => AppSettings.AddOrUpdateValue(IdUseMocks, value);
        }

        public string UrlBase
        {
            get => AppSettings.GetValueOrDefault(IdUrlBase, UrlBaseDefault);
            set => AppSettings.AddOrUpdateValue(IdUrlBase, value);
        }

        public bool UseFakeLocation
        {
            get => AppSettings.GetValueOrDefault(IdUseFakeLocation, UseFakeLocationDefault);
            set => AppSettings.AddOrUpdateValue(IdUseFakeLocation, value);
        }

        public string Latitude
        {
            get => AppSettings.GetValueOrDefault(IdLatitude, FakeLatitudeDefault.ToString());
            set => AppSettings.AddOrUpdateValue(IdLatitude, value);
        }

        public string Longitude
        {
            get => AppSettings.GetValueOrDefault(IdLongitude, FakeLongitudeDefault.ToString());
            set => AppSettings.AddOrUpdateValue(IdLongitude, value);
        }

        public bool AllowGpsLocation
        {
            get => AppSettings.GetValueOrDefault(IdAllowGpsLocation, AllowGpsLocationDefault);
            set => AppSettings.AddOrUpdateValue(IdAllowGpsLocation, value);
        }
    }
}