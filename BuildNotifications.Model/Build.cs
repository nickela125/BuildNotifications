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
        public DateTime QueueTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string BuildUrl { get; set; }
    }
}
