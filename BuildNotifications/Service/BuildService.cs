using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Model;
using BuildNotifications.Model.Message;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace BuildNotifications.Service
{
    public class BuildService : IBuildService
    {
        private readonly IVsoClient _vsoClient;
        private readonly IMessenger _messenger;
        private readonly ISettingsProvider _settingsProvider;

        public BuildService(IVsoClient vsoClient, IMessenger messenger, ISettingsProvider settingsProvider)
        {
            _vsoClient = vsoClient;
            _messenger = messenger;
            _settingsProvider = settingsProvider;

            _messenger.Register<AccountSubscriptionUpdate>(this, UpdateSubscribedBuilds);
        }

        private void UpdateSubscribedBuilds(AccountSubscriptionUpdate update)
        {
            IList<Account> accounts = update.Accounts;
            IList<SubscribedBuild> existingSubscribedBuilds = GetSubscribedBuilds();
            IList<SubscribedBuild> newSubscribedBuilds = new List<SubscribedBuild>();

            foreach (Account account in accounts)
            {
                foreach (Project project in account.Projects)
                {
                    foreach (BuildDefinition buildDefinition in project.Builds)
                    {
                        if (buildDefinition.IsSelected == null || !buildDefinition.IsSelected.Value) continue;

                        SubscribedBuild existingSubscribedBuild =
                            existingSubscribedBuilds.SingleOrDefault(
                                x =>
                                    x.BuildDefinitionId == buildDefinition.Id &&
                                    x.AccountDetails.ProjectId == project.Id &&
                                    x.AccountDetails.AccountName == account.Name);

                        if (existingSubscribedBuild != null)
                        {
                            newSubscribedBuilds.Add(existingSubscribedBuild);
                        }
                        else
                        {
                            newSubscribedBuilds.Add(new SubscribedBuild
                            {
                                AccountDetails = new AccountDetails
                                {
                                    AccountName = account.Name,
                                    ProjectId = project.Id,
                                    EncodedCredentials = account.EncodedCredentials,
                                    ProjectName = project.Name
                                },
                                BuildDefinitionId = buildDefinition.Id,
                                Name = buildDefinition.Name
                            });
                        }
                    }
                }
            }

            SaveSubscribedBuilds(newSubscribedBuilds);
        }

        public bool GetNotifyOnStart()
        {
            return (bool)_settingsProvider.GetSetting(Constants.NotifyOnStartConfigurationName);
        }

        public bool GetNotifyOnFinish()
        {
            return (bool)_settingsProvider.GetSetting(Constants.NotifyOnFinishConfigurationName);
        }

        public void SaveNotifyOptions(bool notifyOnStart, bool notifyOnFinish)
        {
            _settingsProvider.SaveSetting(Constants.NotifyOnStartConfigurationName, notifyOnStart);
            _settingsProvider.SaveSetting(Constants.NotifyOnFinishConfigurationName, notifyOnFinish);

            _messenger.Send(new NotifyOptionsUpdate
            {
                NotifyOnStart = notifyOnStart,
                NotifyOnFinish = notifyOnFinish
            });
        }

        public IList<SubscribedBuild> GetSubscribedBuilds()
        {
            string jsonString = (string)_settingsProvider.GetSetting(Constants.SubscribedBuilds);
            return JsonConvert.DeserializeObject<List<SubscribedBuild>>(jsonString);
        }

        private void SaveSubscribedBuilds(IList<SubscribedBuild> subscribedBuilds)
        {
            string jsonString = JsonConvert.SerializeObject(subscribedBuilds);
            _settingsProvider.SaveSetting(Constants.SubscribedBuilds, jsonString);
            _messenger.Send(new SubscribedBuildsUpdate { SubscribedBuilds = subscribedBuilds });
        }

        public async Task<IList<BuildUpdate>> CheckForUpdatedBuilds(IList<SubscribedBuild> subscribedBuilds)
        {
            List<List<SubscribedBuild>> groupedBuilds = subscribedBuilds.GroupBy(b => new
            {
                b.AccountDetails.AccountName,
                b.AccountDetails.ProjectId
            })
            .Select(grp => grp.ToList())
            .ToList();

            var updates = new List<BuildUpdate>();

            foreach (List<SubscribedBuild> groupedBuild in groupedBuilds)
            {
                IList<Build> buildsForAccount =
                    await
                        _vsoClient.GetBuilds(groupedBuild.First().AccountDetails,
                            groupedBuild.Select(g => g.BuildDefinitionId).ToList());
            
                List<List<Build>> buildsByDefinition = buildsForAccount
                    .GroupBy(b => b.BuildDefinitionId)
                    .Select(grp => grp.ToList())
                    .ToList();

                foreach (List<Build> buildList in buildsByDefinition)
                {
                    if (!buildList.Any()) continue;
                    
                    SubscribedBuild subscribedBuildToUpdate = subscribedBuilds.Single(sb => sb.BuildDefinitionId == buildList.First().BuildDefinitionId);
                    updates.AddRange(CheckForUpdateInternal(buildList, subscribedBuildToUpdate));
                }
            }

            SaveSubscribedBuilds(subscribedBuilds);

            if (!GetNotifyOnStart())
            {
                updates.RemoveAll(u => u.Result == null);
            }

            if (!GetNotifyOnFinish())
            {
                updates.RemoveAll(u => u.Result != null);
            }

            return updates; // todo order by date
        }

        private IList<BuildUpdate> CheckForUpdateInternal(List<Build> buildList, SubscribedBuild subscribedBuild)
        {
            IList<BuildUpdate> updates = new List<BuildUpdate>();

            List<Build> orderedBuilds = buildList.OrderByDescending(b => b.LastChangedDate).ToList();
            Build latestBuild = orderedBuilds.First(); // this method is only called if at least one build
            Build secondLatestBuild = orderedBuilds.Count < 2 ? null : orderedBuilds.Last();

            bool haveNotSeenEitherBuild = subscribedBuild.CurrentBuildId == null ||
                                          (!subscribedBuild.CurrentBuildId.Equals(latestBuild?.Id) &&
                                          !subscribedBuild.CurrentBuildId.Equals(secondLatestBuild?.Id));

            bool oneBuildIsCurrent = subscribedBuild.CurrentBuildId != null &&
                                     (subscribedBuild.CurrentBuildId.Equals(latestBuild.Id) ||
                                     (secondLatestBuild != null &&
                                      subscribedBuild.CurrentBuildId.Equals(secondLatestBuild.Id)));

            Build currentBuild = oneBuildIsCurrent
                ? (subscribedBuild.CurrentBuildId.Equals(latestBuild.Id)
                    ? latestBuild
                    : secondLatestBuild)
                : null;
            
            if (haveNotSeenEitherBuild)
            {
                UpdateSubscribedBuild(subscribedBuild, latestBuild);
                
                if (latestBuild != null && latestBuild.LastChangedDate > DateTime.Now.AddSeconds(-10))
                {
                    updates.Add(CreateUpdate(subscribedBuild, latestBuild));
                }
            }
            else if (oneBuildIsCurrent)
            {
                // Send update if older build changed
                if (currentBuild == secondLatestBuild && secondLatestBuild.Status != subscribedBuild.CurrentBuildStatus)
                {
                    subscribedBuild.LastCompletedBuildResult = secondLatestBuild.Result;
                    UpdateSubscribedBuild(subscribedBuild, secondLatestBuild);
                    updates.Add(CreateUpdate(subscribedBuild, secondLatestBuild));
                }

                if (currentBuild == secondLatestBuild || latestBuild.Status != subscribedBuild.CurrentBuildStatus)
                {
                    UpdateSubscribedBuild(subscribedBuild, latestBuild);
                    updates.Add(CreateUpdate(subscribedBuild, latestBuild));
                }
            }
            return updates;
        }

        private void UpdateSubscribedBuild(SubscribedBuild subscribedBuild, Build build)
        {
            if (subscribedBuild.CurrentBuildStatus != build.Status)
            {
                subscribedBuild.LastBuildStatusChangeTime = build.LastChangedDate;

                if (build.Result != null)
                {
                    subscribedBuild.LastCompletedBuildResult = build.Result;
                    subscribedBuild.LastCompletedBuildRequestedFor = build.RequestedFor;
                    subscribedBuild.LastBuildResultChangeTime = build.LastChangedDate;
                }
            }

            subscribedBuild.CurrentBuildId = build.Id;
            subscribedBuild.CurrentBuildStatus = build.Status;
            subscribedBuild.CurrentBuildRequestedFor = build.RequestedFor;
        }

        private BuildUpdate CreateUpdate(SubscribedBuild subscribedBuild, Build build)
        {
            return new BuildUpdate
            {
                Name = subscribedBuild.Name,
                Id = subscribedBuild.BuildDefinitionId,
                RequestedFor = build.RequestedFor,
                Result = build.Result,
                Status = build.Status
            };
        }
    }
}
