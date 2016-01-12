using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Interface.Service
{
    public interface IMapper
    {
        Project MapToProject(VsoProject project);
        BuildDefinition MapToBuildDefinition(VsoBuildDefinition build);
        Build MapToBuild(VsoBuild build);
    }
}
