namespace BuildNotifications.Model
{
    public class BuildUpdate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RequestedFor { get; set; }
        public BuildStatus Status { get; set; }
        public BuildResult? Result { get; set; }
    }
}
