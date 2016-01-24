using System;
using BuildNotifications.Interface.Service;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Service
{
    public class Mapper : IMapper
    {
        public Project MapToProject(VsoProject project)
        {
            return new Project
            {
                Id = project.Id,
                Name = project.Name
            };
        }

        public BuildDefinition MapToBuildDefinition(VsoBuildDefinition build)
        {
            return new BuildDefinition
            {
                Id = build.Id,
                Name = build.Name
            };
        }

        public Build MapToBuild(VsoBuild build)
        {
            return new Build
            {
                Id = build.Id,
                Result = build.Result == null ? null : (BuildResult?)Enum.Parse(typeof(BuildResult), build.Result, true),
                Status = (BuildStatus)Enum.Parse(typeof(BuildStatus), build.Status, true),
                BuildDefinitionId = build.Definition.Id,
                QueueTime = build.QueueTime,
                StartTime = build.StartTime,
                FinishTime = build.FinishTime,
                LastChangedDate = build.LastChangedDate,
                RequestedFor = build.RequestedFor.DisplayName,
                BuildUrl = build.Links.Self.Href
            };
        }
    }
}
