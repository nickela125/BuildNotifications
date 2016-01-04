namespace BuildNotifications.Model
{
    public class VsoBuildUpdate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public BuildStatus Status { get; set; }
        public BuildResult? Result { get; set; }
    }
}
