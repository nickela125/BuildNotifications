using System.Collections.Generic;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class VsoBuildDefinition : TreeItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public new bool IsSelected { get; set; }
        [JsonIgnore]
        public new string DisplayName => Name;

        public BuildStatus? CurrentBuildStatus { get; set; }
        public string CurrentBuildId { get; set; }
        public BuildResult? LastCompletedBuildResult { get; set; }
    }
}