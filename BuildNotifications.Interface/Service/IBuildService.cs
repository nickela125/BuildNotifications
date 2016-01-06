using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Interface.Service
{
    public interface IBuildService
    {
        Task<IList<VsoBuildUpdate>> CheckForUpdatedBuilds(AccountDetails accountDetails, IList<VsoBuildDefinition> buildDefinitions);
    }
}
