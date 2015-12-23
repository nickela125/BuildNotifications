using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Model;

namespace BuildNotifications.Service
{
    public class BuildService : IBuildService
    {
        private readonly IVsoClient _vsoClient;

        public BuildService(IVsoClient vsoClient)
        {
            _vsoClient = vsoClient;
        }

        public async Task<IList<VsoBuildUpdate>> CheckForUpdatedBuilds(VsoProject vsoProject, string accountName, string encodedCredentials, IList<VsoBuildDefinition> buildDefinitions)
        {
            IList<VsoBuild> builds = await _vsoClient.GetBuilds(vsoProject, accountName, encodedCredentials, buildDefinitions.Select(bd => bd.Id).ToList());
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

            if (buildDefinition.CurrentBuildId == null)
            {
                // No builds recorded yet
                VsoBuild latestBuild = buildList.Count == 1 ?
                    buildList.First() :
                    buildList.OrderByDescending(b => b.QueueTime).First();

                buildDefinition.CurrentBuildId = latestBuild.Id;
                buildDefinition.CurrentBuildStatus = latestBuild.Status;
                buildDefinition.LastCompletedBuildResult = latestBuild.Result;

                // send update if queued in last 10 seconds
                if (latestBuild.QueueTime > DateTime.Now.AddSeconds(-10))
                {
                    updates.Add(new VsoBuildUpdate
                    {
                        Name = buildDefinition.DisplayName,
                        Result = latestBuild.Result,
                        Status = latestBuild.Status
                    });
                }
            }
            else
            {
                // Build recorded
                List<VsoBuild> orderedBuilds = buildList.OrderByDescending(b => b.QueueTime).ToList();
                VsoBuild latestBuild = orderedBuilds.First();
                VsoBuild oldestBuild = orderedBuilds.Count == 1 ? null : orderedBuilds.Last();

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
                            Result = latestBuild.Result,
                            Status = latestBuild.Status
                        });
                    }
                }
                // Older build has been seen before
                else if (oldestBuild != null && buildDefinition.CurrentBuildId.Equals(oldestBuild.Id))
                {
                    if (oldestBuild.Status != buildDefinition.CurrentBuildStatus)
                    {
                        buildDefinition.LastCompletedBuildResult = oldestBuild.Result;

                        updates.Add(new VsoBuildUpdate
                        {
                            Name = buildDefinition.DisplayName,
                            Result = oldestBuild.Result,
                            Status = oldestBuild.Status
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
                        Result = latestBuild.Result,
                        Status = latestBuild.Status
                    });
                }
            }
            return updates;
        }
    }
}
