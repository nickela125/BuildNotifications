using Newtonsoft.Json;

namespace BuildNotifications.Model.DTO
{
    public class VsoBuild
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public string QueueTime { get; set; }
        public VsoBuildDefinition Definition { get; set; }
        public VsoUser RequestedFor { get; set; }
        [JsonProperty(PropertyName = "_links")]
        public VsoBuildLinks Links { get; set; }
    }
}
