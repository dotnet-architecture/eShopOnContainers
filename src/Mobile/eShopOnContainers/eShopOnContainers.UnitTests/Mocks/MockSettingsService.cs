using eShopOnContainers.Core.Services.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopOnContainers.UnitTests.Mocks
{
    public class MockSettingsService : ISettingsService
    {
        IDictionary<string, object> _settings = new Dictionary<string, object>();

        const string AccessToken = "access_token";
        const string IdToken = "id_token";
        const string IdUseMocks = "use_mocks";
        const string IdUrlBase = "url_base";
        const string IdUseFakeLocation = "use_fake_location";
        const string IdLatitude = "latitude";
        const string IdLongitude = "longitude";
        const string IdAllowGpsLocation = "allow_gps_location";
        readonly string AccessTokenDefault = string.Empty;
        readonly string IdTokenDefault = string.Empty;
        readonly bool UseMocksDefault = true;
        readonly bool UseFakeLocationDefault = false;
        readonly bool AllowGpsLocationDefault = false;
        readonly double FakeLatitudeDefault = 47.604610d;
        readonly double FakeLongitudeDefault = -122.315752d;
        readonly string UrlBaseDefault = "https://13.88.8.119";

        public string AuthAccessToken
        {
            get => GetValueOrDefault(AccessToken, AccessTokenDefault);
            set => AddOrUpdateValue(AccessToken, value);
        }

        public string AuthIdToken
        {
            get => GetValueOrDefault(IdToken, IdTokenDefault);
            set => AddOrUpdateValue(IdToken, value);
        }

        public bool UseMocks
        {
            get => GetValueOrDefault(IdUseMocks, UseMocksDefault);
            set => AddOrUpdateValue(IdUseMocks, value);
        }

        public string UrlBase
        {
            get => GetValueOrDefault(IdUrlBase, UrlBaseDefault);
            set => AddOrUpdateValue(IdUrlBase, value);
        }

        public bool UseFakeLocation
        {
            get => GetValueOrDefault(IdUseFakeLocation, UseFakeLocationDefault);
            set => AddOrUpdateValue(IdUseFakeLocation, value);
        }

        public string Latitude
        {
            get => GetValueOrDefault(IdLatitude, FakeLatitudeDefault.ToString());
            set => AddOrUpdateValue(IdLatitude, value);
        }

        public string Longitude
        {
            get => GetValueOrDefault(IdLongitude, FakeLongitudeDefault.ToString());
            set => AddOrUpdateValue(IdLongitude, value);
        }

        public bool AllowGpsLocation
        {
            get => GetValueOrDefault(IdAllowGpsLocation, AllowGpsLocationDefault);
            set => AddOrUpdateValue(IdAllowGpsLocation, value);
        }

        public Task AddOrUpdateValue(string key, bool value) => AddOrUpdateValueInternal(key, value);
        public Task AddOrUpdateValue(string key, string value) => AddOrUpdateValueInternal(key, value);
        public bool GetValueOrDefault(string key, bool defaultValue) => GetValueOrDefaultInternal(key, defaultValue);
        public string GetValueOrDefault(string key, string defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        Task AddOrUpdateValueInternal<T>(string key, T value)
        {
            if (value == null)
            {
                Remove(key);
            }

            _settings[key] = value;
            return Task.Delay(10);
        }

        T GetValueOrDefaultInternal<T>(string key, T defaultValue = default(T))
        {
            object value = null;
            if (_settings.ContainsKey(key))
            {
                value = _settings[key];
            }
            return null != value ? (T)value : defaultValue;
        }

        void Remove(string key)
        {
            if (_settings[key] != null)
            {
                _settings.Remove(key);
            }
        }
    }
}
