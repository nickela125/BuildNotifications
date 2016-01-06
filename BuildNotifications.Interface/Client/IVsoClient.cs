using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Interface.Client
{
    public interface IVsoClient
    {
        Task<IList<VsoProject>> GetProjects(AccountDetails accountDetails);
        Task<IList<VsoBuildDefinition>> GetBuildDefinitions(AccountDetails accountDetails);
        Task<IList<VsoBuild>> GetBuilds(AccountDetails accountDetails, IList<string> buildDefinitions);
    }
}
