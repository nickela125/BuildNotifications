using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Interface.Client
{
    public interface IVsoClient
    {
        Task<IList<Project>> GetProjects(AccountDetails accountDetails);
        Task<IList<BuildDefinition>> GetBuildDefinitions(AccountDetails accountDetails);
        Task<IList<Build>> GetBuilds(AccountDetails accountDetails, IList<string> buildDefinitions);
    }
}
