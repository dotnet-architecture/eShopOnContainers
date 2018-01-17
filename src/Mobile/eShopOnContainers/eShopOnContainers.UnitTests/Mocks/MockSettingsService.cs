using eShopOnContainers.Core.Services.Settings;
using System;

namespace eShopOnContainers.UnitTests.Mocks
{
    public class MockSettingsService : ISettingsService
    {
        string _accessTokenDefault = string.Empty;
        string _idTokenDefault = string.Empty;
        bool _useMocksDefault = true;
        string _urlBaseDefault = "https://13.88.8.119";
        bool _useFakeLocationDefault = false;
        bool _allowGpsLocationDefault = false;
        double _fakeLatitudeDefault = 47.604610d;
        double _fakeLongitudeDefault = -122.315752d;

        public string AuthAccessToken
        {
            get { return _accessTokenDefault; }
            set { _accessTokenDefault = value; }
        }

        public string AuthIdToken
        {
            get { return _idTokenDefault; }
            set { _idTokenDefault = value; }
        }

        public bool UseMocks
        {
            get { return _useMocksDefault; }
            set { _useMocksDefault = value; }
        }

        public string UrlBase
        {
            get { return _urlBaseDefault; }
            set { _urlBaseDefault = value; }
        }

        public bool UseFakeLocation
        {
            get { return _useFakeLocationDefault; }
            set { _useFakeLocationDefault = value; }
        }

        public string Latitude
        {
            get { return _fakeLatitudeDefault.ToString(); }
            set { _fakeLatitudeDefault = Convert.ToDouble(value); }
        }

        public string Longitude
        {
            get { return _fakeLongitudeDefault.ToString(); }
            set { _fakeLongitudeDefault = Convert.ToDouble(value); }
        }

        public bool AllowGpsLocation
        {
            get { return _allowGpsLocationDefault; }
            set { _allowGpsLocationDefault = value; }
        }
    }
}
