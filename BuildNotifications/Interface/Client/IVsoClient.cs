using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Model;

namespace BuildNotifications.Interface.Client
{
    public interface IVsoClient
    {
        Task<IList<VsoProject>> GetProjects(VsoAccount account);
        Task<List<VsoBuild>> GetBuilds(VsoProject project, VsoAccount account);
    }
}
