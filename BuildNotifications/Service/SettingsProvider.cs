using BuildNotifications.Interface.Service;

namespace BuildNotifications.Service
{
    public class SettingsProvider : ISettingsProvider
    {
        public object GetSetting(string settingName)
        {
            return Properties.Settings.Default[settingName];
        }

        public void SaveSetting(string settingName, object settingValue)
        {
            Properties.Settings.Default[settingName] = settingValue;
            Properties.Settings.Default.Save();
        }
    }
}
