using System.Collections.Generic;

namespace BuildNotifications.Model.Message
{
    public class SubscribedBuildsUpdate
    {
        public IList<SubscribedBuild> SubscribedBuilds { get; set; }
    }
}
