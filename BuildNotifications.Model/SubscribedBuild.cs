using GalaSoft.MvvmLight;

namespace BuildNotifications.Model
{
    public class SubscribedBuild : ViewModelBase
    {
        public AccountDetails AccountDetails { get; set; }
        public string BuildDefinitionId { get; set; }
        public string DisplayName{ get; set; }
        public BuildStatus? CurrentBuildStatus { get; set; }
        public string CurrentBuildId { get; set; }

        private BuildResult? _lastCompletedBuildResult;

        public BuildResult? LastCompletedBuildResult
        {
            get { return _lastCompletedBuildResult; }
            set { Set(ref _lastCompletedBuildResult, value); }
        }
    }
}
