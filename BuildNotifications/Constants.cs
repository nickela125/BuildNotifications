namespace BuildNotifications
{
    public static class Constants
    {
        // Settings Names
        public const string AccountsConfigurationName = "Accounts";
        public const string NotifyOnStartConfigurationName = "NotifyOnStart";
        public const string NotifyOnFinishConfigurationName = "NotifyOnFinish";

        // URIs
        public const string VsoBaseAddress = "https://{0}.visualstudio.com/defaultcollection/";
        public const string VsoBuildDefinitionsAddress = "{0}/_apis/build/definitions?api-version=2.0";
        public const string VsoProjectsAddress = "_apis/projects?api-version=1.0";
    }
}
