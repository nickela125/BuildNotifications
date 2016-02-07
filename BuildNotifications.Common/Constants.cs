namespace BuildNotifications.Common
{
    public static class Constants
    {
        public const string AppId = "Crawford.BuildNotifications";

        // Settings Names
        public const string AccountsConfigurationName = "Accounts";
        public const string NotifyOnStartConfigurationName = "NotifyOnStart";
        public const string NotifyOnFinishConfigurationName = "NotifyOnFinish";
        public const string SubscribedBuilds = "SubscribedBuilds";

        // URIs
        public const string VsoBaseAddress = "https://{0}.visualstudio.com/defaultcollection/";
        public const string VsoBuildDefinitionsAddress = "{0}/_apis/build/definitions?api-version=2.0";
        public const string VsoProjectsAddress = "_apis/projects?api-version=1.0";
        public const string VsoBuildsAddress = "{0}/_apis/build/builds?definitions={1}&api-version=2.0&maxBuildsPerDefinition={2}";

        // Filter Status Options
        public const string FilterByResult = "Complete Build Result";
        public const string FilterByStatus = "Current Build Status";
        public const string BuildStatusPropertyName = "CurrentBuildStatus";
        public const string BuildResultPropertyName = "LastCompletedBuildResult";
    }
}
