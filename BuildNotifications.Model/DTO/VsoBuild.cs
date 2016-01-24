using System;
using Newtonsoft.Json;

namespace BuildNotifications.Model.DTO
{
    public class VsoBuild
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public DateTime QueueTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public DateTime LastChangedDate { get; set; }
        public VsoBuildDefinition Definition { get; set; }
        public VsoUser RequestedFor { get; set; }
        [JsonProperty(PropertyName = "_links")]
        public VsoBuildLinks Links { get; set; }
    }
}
