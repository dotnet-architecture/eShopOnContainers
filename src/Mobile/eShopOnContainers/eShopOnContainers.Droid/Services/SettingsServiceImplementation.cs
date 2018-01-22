using Android.App;
using Android.Content;
using Android.Preferences;
using eShopOnContainers.Core.Services.Settings;
using eShopOnContainers.Droid.Services;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(SettingsServiceImplementation))]
namespace eShopOnContainers.Droid.Services
{
    public class SettingsServiceImplementation : ISettingsServiceImplementation
    {
        #region Internal Implementation

        readonly object _locker = new object();

        ISharedPreferences GetSharedPreference()
        {
            return PreferenceManager.GetDefaultSharedPreferences(Application.Context);
        }

        bool AddOrUpdateValueInternal<T>(string key, T value)
        {
            if (Application.Context == null)
                return false;

            if (value == null)
            {
                Remove(key);
                return true;
            }

            var type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }
            var typeCode = Type.GetTypeCode(type);

            lock (_locker)
            {
                using (var sharedPrefs = GetSharedPreference())
                {
                    using (var editor = sharedPrefs.Edit())
                    {
                        switch (typeCode)
                        {
                            case TypeCode.Boolean:
                                editor.PutBoolean(key, Convert.ToBoolean(value));
                                break;
                            case TypeCode.String:
                                editor.PutString(key, Convert.ToString(value));
                                break;
                            default:
                                throw new ArgumentException($"Value of type {typeCode} is not supported.");
                        }
                        editor.Commit();
                    }
                }
            }
            return true;
        }

        T GetValueOrDefaultInternal<T>(string key, T defaultValue = default(T))
        {
            if (Application.Context == null)
                return defaultValue;

            if (!Contains(key))
                return defaultValue;

            lock (_locker)
            {
                using (var sharedPrefs = GetSharedPreference())
                {
                    var type = typeof(T);
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }

                    object value = null;
                    var typeCode = Type.GetTypeCode(type);
                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            value = sharedPrefs.GetBoolean(key, Convert.ToBoolean(defaultValue));
                            break;
                        case TypeCode.String:
                            value = sharedPrefs.GetString(key, Convert.ToString(defaultValue));
                            break;
                        default:
                            throw new ArgumentException($"Value of type {typeCode} is not supported.");
                    }

                    return null != value ? (T)value : defaultValue;
                }
            }
        }

        bool Contains(string key)
        {
            if (Application.Context == null)
                return false;

            lock (_locker)
            {
                using (var sharedPrefs = GetSharedPreference())
                {
                    if (sharedPrefs == null)
                        return false;
                    return sharedPrefs.Contains(key);
                }
            }
        }

        #endregion

        #region ISettingsServiceImplementation

        public bool AddOrUpdateValue(string key, bool value) => AddOrUpdateValueInternal(key, value);

        public bool AddOrUpdateValue(string key, string value) => AddOrUpdateValueInternal(key, value);

        public bool GetValueOrDefault(string key, bool defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public string GetValueOrDefault(string key, string defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public void Remove(string key)
        {
            if (Application.Context == null)
                return;

            lock (_locker)
            {
                using (var sharedPrefs = GetSharedPreference())
                {
                    using (var editor = sharedPrefs.Edit())
                    {
                        editor.Remove(key);
                        editor.Commit();
                    }
                }
            }
        }

        #endregion
    }
}
