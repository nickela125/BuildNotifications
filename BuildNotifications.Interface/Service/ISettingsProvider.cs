namespace BuildNotifications.Interface.Service
{
    public interface ISettingsProvider
    {
        object GetSetting(string settingName);
        void SaveSetting(string settingName, object settingValue);
    }
}
