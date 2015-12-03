using BuildNotifications.Interface.Service;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Service
{
    public class Mapper : IMapper
    {
        public VsoProject MapToVsoProject(Project project)
        {
            return new VsoProject
            {
                Id = project.Id,
                Name = project.Name
            };
        }

        public VsoBuild MapToVsoBuild(Build build)
        {
            return new VsoBuild
            {
                Id = build.Id,
                Name = build.Name
            };
        }
    }
}
