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

        public BuildService(IVsoClient vsoClient, IMessenger messenger)
        {
            _vsoClient = vsoClient;
            _messenger = messenger;

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
                        if (buildDefinition.IsSelected != null && buildDefinition.IsSelected.Value)
                        {
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
            }

            SaveSubscribedBuilds(newSubscribedBuilds);
        }

        public bool GetNotifyOnStart()
        {
            return (bool)Properties.Settings.Default[Constants.NotifyOnStartConfigurationName];
        }

        public bool GetNotifyOnFinish()
        {
            return (bool)Properties.Settings.Default[Constants.NotifyOnFinishConfigurationName];
        }

        public void SaveNotifyOptions(bool notifyOnStart, bool notifyOnFinish)
        {
            Properties.Settings.Default[Constants.NotifyOnStartConfigurationName] = notifyOnStart;
            Properties.Settings.Default[Constants.NotifyOnFinishConfigurationName] = notifyOnFinish;
            Properties.Settings.Default.Save();

            _messenger.Send<NotifyOptionsUpdate>(new NotifyOptionsUpdate
            {
                NotifyOnStart = notifyOnStart,
                NotifyOnFinish = notifyOnFinish
            });
        }

        public IList<SubscribedBuild> GetSubscribedBuilds()
        {
            string jsonString = (string)Properties.Settings.Default[Constants.SubscribedBuilds];

            return JsonConvert.DeserializeObject<List<SubscribedBuild>>(jsonString);
        }

        private void SaveSubscribedBuilds(IList<SubscribedBuild> subscribedBuilds)
        {
            string jsonString = JsonConvert.SerializeObject(subscribedBuilds);
            Properties.Settings.Default[Constants.SubscribedBuilds] = jsonString;
            Properties.Settings.Default.Save();
            _messenger.Send(new SubscribedBuildsUpdate {SubscribedBuilds = subscribedBuilds});
        }

        public async Task<IList<BuildUpdate>> CheckForUpdatedBuilds(IList<SubscribedBuild> subscribedBuilds)
        {
            // todo group by account details to reduce calls

            var updates = new List<BuildUpdate>();

            foreach (SubscribedBuild subscribedBuild in subscribedBuilds)
            {
                IList<Build> builds = await _vsoClient.GetBuilds(subscribedBuild.AccountDetails, new []{ subscribedBuild.BuildDefinitionId });
                List<List<Build>> buildsByDefinition = builds
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
            Build latestBuild = orderedBuilds.First(); // method only called if at least one build
            Build secondLatestBuild = orderedBuilds.Count < 2 ? null : orderedBuilds.Last();

            if (subscribedBuild.CurrentBuildId == null || 
                    (!subscribedBuild.CurrentBuildId.Equals(latestBuild.Id) && 
                        (secondLatestBuild == null || !subscribedBuild.CurrentBuildId.Equals(secondLatestBuild.Id))))
            {
                // One or two builds that we haven't seen before

                subscribedBuild.CurrentBuildId = latestBuild.Id;
                subscribedBuild.CurrentBuildStatus = latestBuild.Status;
                subscribedBuild.LastCompletedBuildResult = latestBuild.Result;
                subscribedBuild.LastCompletedBuildRequestedFor = latestBuild.RequestedFor;

                // send update if queued in last 10 seconds
                if (latestBuild.LastChangedDate > DateTime.Now.AddSeconds(-10))
                {
                    updates.Add(new BuildUpdate
                    {
                        Name = subscribedBuild.Name,
                        Id = subscribedBuild.BuildDefinitionId,
                        RequestedFor = latestBuild.RequestedFor,
                        Result = latestBuild.Result,
                        Status = latestBuild.Status
                    });
                }
            }
            else
            {
                // latest build already seen before
                if (subscribedBuild.CurrentBuildId.Equals(latestBuild.Id))
                {
                    if (latestBuild.Result != null && latestBuild.Status != subscribedBuild.CurrentBuildStatus)
                    {
                        subscribedBuild.CurrentBuildStatus = latestBuild.Status;
                        subscribedBuild.LastCompletedBuildResult = latestBuild.Result;
                        subscribedBuild.LastCompletedBuildRequestedFor = latestBuild.RequestedFor;

                        updates.Add(new BuildUpdate
                        {
                            Name = subscribedBuild.Name,
                            Id = subscribedBuild.BuildDefinitionId,
                            RequestedFor = latestBuild.RequestedFor,
                            Result = latestBuild.Result,
                            Status = latestBuild.Status
                        });
                    }
                    else if (latestBuild.Status != subscribedBuild.CurrentBuildStatus)
                    {
                        subscribedBuild.CurrentBuildStatus = latestBuild.Status;
                        
                        updates.Add(new BuildUpdate
                        {
                            Name = subscribedBuild.Name,
                            Id = subscribedBuild.BuildDefinitionId,
                            RequestedFor = latestBuild.RequestedFor,
                            Result = latestBuild.Result,
                            Status = latestBuild.Status
                        });
                    }
                }
                // Second latest build has been seen before
                else if (secondLatestBuild != null && subscribedBuild.CurrentBuildId.Equals(secondLatestBuild.Id))
                {
                    if (secondLatestBuild.Status != subscribedBuild.CurrentBuildStatus)
                    {
                        subscribedBuild.LastCompletedBuildResult = secondLatestBuild.Result;

                        updates.Add(new BuildUpdate
                        {
                            Name = subscribedBuild.Name,
                            Id = subscribedBuild.BuildDefinitionId,
                            RequestedFor = secondLatestBuild.RequestedFor,
                            Result = secondLatestBuild.Result,
                            Status = secondLatestBuild.Status
                        });
                    }
                    
                    // look at new build
                    subscribedBuild.CurrentBuildStatus = latestBuild.Status;
                    subscribedBuild.CurrentBuildId = latestBuild.Id;

                    if (latestBuild.Result != null)
                    {
                        subscribedBuild.LastCompletedBuildResult = latestBuild.Result;
                        subscribedBuild.LastCompletedBuildRequestedFor = latestBuild.RequestedFor;
                    }

                    updates.Add(new BuildUpdate
                    {
                        Name = subscribedBuild.Name,
                        Id = subscribedBuild.BuildDefinitionId,
                        RequestedFor = latestBuild.RequestedFor,
                        Result = latestBuild.Result,
                        Status = latestBuild.Status
                    });
                }
            }
            return updates;
        }

        private void UpdateSubscribedBuild(SubscribedBuild subscribedBuild, Build lastestBuild)
        {
            
        }
    }
}
