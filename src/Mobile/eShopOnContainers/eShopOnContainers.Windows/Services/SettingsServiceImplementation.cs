using eShopOnContainers.Core.Services.Settings;
using eShopOnContainers.Windows.Services;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(SettingsServiceImplementation))]
namespace eShopOnContainers.Windows.Services
{
    public class SettingsServiceImplementation : ISettingsServiceImplementation
    {
        #region Internal Implementation

        readonly object _locker = new object();

        ApplicationDataContainer GetAppSettings()
        {
            return ApplicationData.Current.LocalSettings;
        }

        bool AddOrUpdateValueInternal<T>(string key, T value)
        {
            bool valueChanged = false;

            if (value == null)
            {
                Remove(key);
                return true;
            }

            lock (_locker)
            {
                var settings = GetAppSettings();
                if (settings.Values.ContainsKey(key))
                {
                    if (settings.Values[key] != (object)value)
                    {
                        settings.Values[key] = value;
                        valueChanged = true;
                    }
                }
                else
                {
                    settings.Values[key] = value;
                    valueChanged = true;
                }
            }

            return valueChanged;
        }

        T GetValueOrDefaultInternal<T>(string key, T defaultValue = default(T))
        {
            object value;

            lock (_locker)
            {
                var settings = GetAppSettings();
                if (settings.Values.ContainsKey(key))
                {
                    var tempValue = settings.Values[key];
                    if (tempValue != null)
                        value = (T)tempValue;
                    else
                        value = defaultValue;
                }
                else
                {
                    value = defaultValue;
                }
            }
            return null != value ? (T)value : defaultValue;
        }

        #endregion

        #region ISettingsServiceImplementation

        public bool AddOrUpdateValue(string key, bool value) => AddOrUpdateValueInternal(key, value);

        public bool AddOrUpdateValue(string key, string value) => AddOrUpdateValueInternal(key, value);

        public bool GetValueOrDefault(string key, bool defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public string GetValueOrDefault(string key, string defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public void Remove(string key)
        {
            lock (_locker)
            {
                var settings = GetAppSettings();
                if (settings.Values.ContainsKey(key))
                {
                    settings.Values.Remove(key);
                }
            }
        }

        #endregion
    }
}
