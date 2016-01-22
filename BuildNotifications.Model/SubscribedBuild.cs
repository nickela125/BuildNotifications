using System;
using GalaSoft.MvvmLight;

namespace BuildNotifications.Model
{
    public class SubscribedBuild : ViewModelBase
    {
        public AccountDetails AccountDetails { get; set; }
        public string BuildDefinitionId { get; set; }
        public string Name{ get; set; }
        public string LastCompletedBuildRequestedFor{ get; set; }
        public string CurrentBuildRequestedFor{ get; set; }
        public DateTime LastBuildResultChangeTime{ get; set; }
        public DateTime LastBuildStatusChangeTime{ get; set; }
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
