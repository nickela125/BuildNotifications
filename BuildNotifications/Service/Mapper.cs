using System;
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

        public VsoBuildDefinition MapToVsoBuildDefinition(BuildDefinition build)
        {
            return new VsoBuildDefinition
            {
                Id = build.Id,
                Name = build.Name
            };
        }

        public VsoBuild MapToVsoBuild(Build build)
        {
            return new VsoBuild
            {
                Id = build.Id,
                Result = build.Result == null ? null : (BuildResult?)Enum.Parse(typeof(BuildResult), build.Result, true),
                Status = (BuildStatus)Enum.Parse(typeof(BuildStatus), build.Status, true),
                BuildDefinitionId = build.Definition.Id,
                QueueTime = DateTime.Parse(build.QueueTime),
                RequestedFor = build.RequestedFor.DisplayName
            };
        }
    }
}
