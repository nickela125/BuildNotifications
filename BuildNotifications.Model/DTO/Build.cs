using Newtonsoft.Json;

namespace BuildNotifications.Model.DTO
{
    public class Build
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public string QueueTime { get; set; }
        public BuildDefinition Definition { get; set; }
        public User RequestedFor { get; set; }
        [JsonProperty(PropertyName = "_links")]
        public BuildLinks Links { get; set; }
    }
}
