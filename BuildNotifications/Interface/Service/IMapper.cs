using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Interface.Service
{
    public interface IMapper
    {
        VsoProject MapToVsoProject(Project project);
        VsoBuild MapToVsoBuild(Build build);
    }
}
