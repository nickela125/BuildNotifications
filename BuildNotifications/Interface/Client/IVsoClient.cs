using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Interface.Client
{
    public interface IVsoClient
    {
        Task<IList<VsoProject>> GetProjects(string accountName, string encodedCredentials);
        Task<IList<VsoBuildDefinition>> GetBuildDefinitions(VsoProject project, string accountName, string encodedCredentials);
        Task<IList<VsoBuild>> GetBuilds(VsoProject project, string accountName, string encodedCredentials, IList<string> buildDefinitions);
    }
}
