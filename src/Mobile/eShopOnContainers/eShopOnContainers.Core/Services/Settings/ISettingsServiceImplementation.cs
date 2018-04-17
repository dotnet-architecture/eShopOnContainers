namespace eShopOnContainers.Core.Services.Settings
{
    public interface ISettingsServiceImplementation
    {
        bool GetValueOrDefault(string key, bool defaultValue);
        string GetValueOrDefault(string key, string defaultValue);

        bool AddOrUpdateValue(string key, bool value);
        bool AddOrUpdateValue(string key, string value);

        void Remove(string key);
    }
}
