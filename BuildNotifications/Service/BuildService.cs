using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Service
{
    public class BuildService : IBuildService
    {
        private readonly IVsoClient _vsoClient;

        public BuildService(IVsoClient vsoClient)
        {
            _vsoClient = vsoClient;
        }

        public async Task<IList<VsoBuildUpdate>> CheckForUpdatedBuilds(AccountDetails accountDetails, IList<VsoBuildDefinition> buildDefinitions)
        {
            IList<VsoBuild> builds = await _vsoClient.GetBuilds(accountDetails, buildDefinitions.Select(bd => bd.Id).ToList());
            List<List<VsoBuild>> buildsByDefinition = builds
                .GroupBy(b => b.BuildDefinitionId)
                .Select(grp => grp.ToList())
                .ToList();

            var updates = new List<VsoBuildUpdate>();

            foreach (List<VsoBuild> buildList in buildsByDefinition)
            {
                if (buildList.Any())
                {
                    VsoBuildDefinition buildDefinition = buildDefinitions.Single(bd => bd.Id == buildList.First().BuildDefinitionId);
                    updates.AddRange(CheckForUpdateInternal(buildList, buildDefinition));
                }
            }
            return updates;
        }

        private IList<VsoBuildUpdate> CheckForUpdateInternal(List<VsoBuild> buildList, VsoBuildDefinition buildDefinition)
        {
            IList<VsoBuildUpdate> updates = new List<VsoBuildUpdate>();
            
            List<VsoBuild> orderedBuilds = buildList.OrderByDescending(b => b.QueueTime).ToList();
            VsoBuild latestBuild = orderedBuilds.First(); // method only called if at least one build
            VsoBuild secondLatestBuild = orderedBuilds.Count < 2 ? null : orderedBuilds.Last();

            if (buildDefinition.CurrentBuildId == null || 
                    (!buildDefinition.CurrentBuildId.Equals(latestBuild.Id) && 
                        (secondLatestBuild == null || !buildDefinition.CurrentBuildId.Equals(secondLatestBuild.Id))))
            {
                // One or two builds that we haven't seen before

                buildDefinition.CurrentBuildId = latestBuild.Id;
                buildDefinition.CurrentBuildStatus = latestBuild.Status;
                buildDefinition.LastCompletedBuildResult = latestBuild.Result;

                // send update if queued in last 10 seconds
                if (latestBuild.QueueTime > DateTime.Now.AddSeconds(-10))
                {
                    updates.Add(new VsoBuildUpdate
                    {
                        Name = buildDefinition.DisplayName,
                        Id = buildDefinition.Id,
                        RequestedFor = latestBuild.RequestedFor,
                        Result = latestBuild.Result,
                        Status = latestBuild.Status
                    });
                }
            }
            else
            {
                // latest build already seen before
                if (buildDefinition.CurrentBuildId.Equals(latestBuild.Id))
                {
                    if (latestBuild.Result != null && latestBuild.Status != buildDefinition.CurrentBuildStatus)
                    {
                        buildDefinition.CurrentBuildStatus = latestBuild.Status;
                        buildDefinition.LastCompletedBuildResult = latestBuild.Result;

                        updates.Add(new VsoBuildUpdate
                        {
                            Name = buildDefinition.DisplayName,
                            Id = buildDefinition.Id,
                            RequestedFor = latestBuild.RequestedFor,
                            Result = latestBuild.Result,
                            Status = latestBuild.Status
                        });
                    }
                    else if (latestBuild.Status != buildDefinition.CurrentBuildStatus)
                    {
                        buildDefinition.CurrentBuildStatus = latestBuild.Status;
                        
                        updates.Add(new VsoBuildUpdate
                        {
                            Name = buildDefinition.DisplayName,
                            Id = buildDefinition.Id,
                            RequestedFor = latestBuild.RequestedFor,
                            Result = latestBuild.Result,
                            Status = latestBuild.Status
                        });
                    }
                }
                // Second latest build has been seen before
                else if (secondLatestBuild != null && buildDefinition.CurrentBuildId.Equals(secondLatestBuild.Id))
                {
                    if (secondLatestBuild.Status != buildDefinition.CurrentBuildStatus)
                    {
                        buildDefinition.LastCompletedBuildResult = secondLatestBuild.Result;

                        updates.Add(new VsoBuildUpdate
                        {
                            Name = buildDefinition.DisplayName,
                            Id = buildDefinition.Id,
                            RequestedFor = secondLatestBuild.RequestedFor,
                            Result = secondLatestBuild.Result,
                            Status = secondLatestBuild.Status
                        });
                    }
                    
                    // look at new build
                    buildDefinition.CurrentBuildStatus = latestBuild.Status;
                    buildDefinition.CurrentBuildId = latestBuild.Id;

                    if (latestBuild.Result != null)
                    {
                        buildDefinition.LastCompletedBuildResult = latestBuild.Result;
                    }

                    updates.Add(new VsoBuildUpdate
                    {
                        Name = buildDefinition.DisplayName,
                        Id = buildDefinition.Id,
                        RequestedFor = latestBuild.RequestedFor,
                        Result = latestBuild.Result,
                        Status = latestBuild.Status
                    });
                }
            }
            return updates;
        }
    }
}
