using System;
using Foundation;
using eShopOnContainers.Core.Services.Settings;
using eShopOnContainers.iOS.Services;
using System.Globalization;

[assembly: Xamarin.Forms.Dependency(typeof(SettingsServiceImplementation))]
namespace eShopOnContainers.iOS.Services
{
    public class SettingsServiceImplementation : ISettingsServiceImplementation
    {
        readonly object locker = new object();

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

            lock (locker)
            {
                var defaults = GetUserDefaults();
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        defaults.SetString(Convert.ToString(value, CultureInfo.InvariantCulture), key);
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

        #region ISettingsService Implementation

        public bool AddOrUpdateValue(string key, bool value) => AddOrUpdateValueInternal(key, value);

        public bool AddOrUpdateValue(string key, string value) => AddOrUpdateValueInternal(key, value);

        public bool GetValueOrDefault(string key, bool defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public string GetValueOrDefault(string key, string defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public void Remove(string key)
        {
            lock (locker)
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

        public void Clear()
        {
            lock (locker)
            {
                var defaults = GetUserDefaults();
                try
                {
                    var items = defaults.ToDictionary();
                    foreach (var item in items.Keys)
                    {
                        if (item is NSString nsString)
                        {
                            defaults.RemoveObject(nsString);
                        }
                    }
                    defaults.Synchronize();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to clear all defaults. Message: " + ex.Message);
                }
            }
        }

        public bool Contains(string key)
        {
            lock (locker)
            {
                var defaults = GetUserDefaults();
                try
                {
                    var setting = defaults[key];
                    return setting != null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to clear all defaults. Message: " + ex.Message);
                }
                return false;
            }
        }

        #endregion
    }
}
