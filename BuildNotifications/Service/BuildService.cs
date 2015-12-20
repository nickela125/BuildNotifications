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
                    VsoBuildDefinition buildDefinition = buildDefinitions.Single(bd => bd.Id == buildList.First().Id);
                    updates.AddRange(CheckForUpdateInternal(buildList, buildDefinition));
                }
            }
            return updates;
        }

        private IList<VsoBuildUpdate> CheckForUpdateInternal(List<VsoBuild> buildList, VsoBuildDefinition buildDefinition)
        {
            IList<VsoBuildUpdate> updates = new List<VsoBuildUpdate>();
            if (buildDefinition.LastCompletedBuildResult != null)
            {
                return updates;
            }

            if (buildDefinition.CurrentBuildId == null)
            {
                // No builds recorded yet
                VsoBuild latestBuild = buildList.Count == 1 ?
                    buildList.First() :
                    buildList.OrderByDescending(b => b.QueueTime).First();

                buildDefinition.Id = latestBuild.Id;
                buildDefinition.CurrentBuildStatus = latestBuild.Status;

                // send update if queued in last 10 seconds
                if (latestBuild.Result != null && latestBuild.QueueTime < DateTime.Now.AddSeconds(-10))
                {
                    // TODO send notification
                }
            }
            else
            {
                // Build recorded
                List<VsoBuild> orderedBuilds = buildList.OrderByDescending(b => b.QueueTime).ToList();
                VsoBuild latestBuild = orderedBuilds.First();
                VsoBuild oldestBuild = orderedBuilds.Count == 1 ? null : orderedBuilds.Last();


                if (buildDefinition.CurrentBuildId.Equals(latestBuild.Id))
                {
                    if (latestBuild.Result != null)
                    {
                        buildDefinition.CurrentBuildStatus = latestBuild.Status;
                        buildDefinition.LastCompletedBuildResult = latestBuild.Result;
                        // TODO send notification
                    }
                    else if (latestBuild.Status != buildDefinition.CurrentBuildStatus)
                    {
                        buildDefinition.CurrentBuildStatus = latestBuild.Status;
                        // TODO send notification
                    }
                }
                else if (oldestBuild != null && buildDefinition.CurrentBuildId.Equals(oldestBuild.Id))
                {
                    if (oldestBuild.Result != buildDefinition.LastCompletedBuildResult)
                    {
                        // TODO send notification
                    }
                    else if (latestBuild.Status != buildDefinition.CurrentBuildStatus)
                    {
                        buildDefinition.CurrentBuildStatus = latestBuild.Status;
                        // TODO send notification
                    }
                }
            }
            return updates;
        }
    }
}
