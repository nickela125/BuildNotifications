using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Model;

namespace BuildNotifications.Interface.Service
{
    public interface IAccountService
    {
        Task UpdateAccount(VsoAccount account);
        IList<VsoAccount> GetAccounts();
        void SaveAccounts(IList<VsoAccount> accounts);
        void UpdateBuildDefinitions(IList<VsoSubscibedBuildList> subscibedBuilds);
        void RemoveAccount(VsoAccount account);
        bool GetNotifyOnStart();
        bool GetNotifyOnFinish();
        void SaveNotifyOptions(bool notifyOnStart, bool notifyOnFinish);
        void UpdateBuildStatus(string accountName, string projectId, IList<VsoBuildDefinition> updatedDefinitions);
    }
}
