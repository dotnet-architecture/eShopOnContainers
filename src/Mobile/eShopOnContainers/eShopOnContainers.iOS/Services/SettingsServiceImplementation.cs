using eShopOnContainers.Core.Services.Settings;
using eShopOnContainers.iOS.Services;
using Foundation;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(SettingsServiceImplementation))]
namespace eShopOnContainers.iOS.Services
{
    public class SettingsServiceImplementation : ISettingsServiceImplementation
    {
        #region Internal Implementation

        readonly object _locker = new object();

        NSUserDefaults GetUserDefaults() => NSUserDefaults.StandardUserDefaults;

        bool AddOrUpdateValueInternal<T>(string key, T value)
        {
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
                var defaults = GetUserDefaults();
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        defaults.SetBool(Convert.ToBoolean(value), key);
                        break;
                    case TypeCode.String:
                        defaults.SetString(Convert.ToString(value), key);
                        break;
                    default:
                        throw new ArgumentException($"Value of type {typeCode} is unsupported.");
                }

                try
                {
                    defaults.Synchronize();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
                }
            }
            return true;
        }

        T GetValueOrDefaultInternal<T>(string key, T defaultValue = default(T))
        {
            lock (_locker)
            {
                var defaults = GetUserDefaults();

                if (defaults[key] == null)
                {
                    return defaultValue;
                }

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
                        value = defaults.BoolForKey(key);
                        break;
                    case TypeCode.String:
                        value = defaults.StringForKey(key);
                        break;
                    default:
                        throw new ArgumentException($"Value of type {typeCode} is unsupported.");
                }

                return null != value ? (T)value : defaultValue;
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
            lock (_locker)
            {
                var defaults = GetUserDefaults();
                try
                {
                    if (defaults[key] != null)
                    {
                        defaults.RemoveObject(key);
                        defaults.Synchronize();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to remove: " + key, " Message: " + ex.Message);
                }
            }
        }

        #endregion
    }
}
