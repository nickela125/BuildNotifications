using System;

namespace BuildNotifications.Model
{
    public class Build
    {
        public string Id { get; set; }
        public string BuildDefinitionId { get; set; }
        public string RequestedFor { get; set; }
        public BuildResult? Result { get; set; }
        public BuildStatus Status { get; set; }
        public DateTime LastChangedDate { get; set; }
        public string BuildUrl { get; set; }
    }
}
