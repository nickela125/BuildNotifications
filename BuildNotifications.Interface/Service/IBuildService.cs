using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Interface.Service
{
    public interface IBuildService
    {
        bool GetNotifyOnStart();
        bool GetNotifyOnFinish();
        void SaveNotifyOptions(bool notifyOnStart, bool notifyOnFinish);
        IList<SubscribedBuild> GetSubscribedBuilds();
        Task<IList<BuildUpdate>> CheckForUpdatedBuilds(IList<SubscribedBuild> subscribedBuilds);
    }
}
