using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Interface.Service
{
    public interface IMapper
    {
        VsoProject MapToVsoProject(Project project);
        VsoBuildDefinition MapToVsoBuildDefinition(BuildDefinition build);
        VsoBuild MapToVsoBuild(Build build);
    }
}
