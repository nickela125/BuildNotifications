using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Model;
using BuildNotifications.Service;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BuildNotifications.Test.Service
{
    [TestClass]
    public class BuildServiceTests
    {
        // todo rewrite tests
        private IBuildService InitializeBuildService(IList<Build> builds)
        {
            var mockVsoClient = new Mock<IVsoClient>();
            mockVsoClient.Setup(x =>
                x.GetBuilds(It.IsAny<AccountDetails>(), It.IsAny<IList<string>>()))
                .Returns(Task.FromResult(builds));

            var mockSettingsProvider = new Mock<ISettingsProvider>();
            mockSettingsProvider.Setup(x =>
                x.GetSetting(Constants.NotifyOnStartConfigurationName))
                .Returns(true);
            mockSettingsProvider.Setup(x =>
                x.GetSetting(Constants.NotifyOnFinishConfigurationName))
                .Returns(true);

            return new BuildService(mockVsoClient.Object, new Messenger(), mockSettingsProvider.Object);
        }

        private AccountDetails CreateAccountDetails()
        {
            return new AccountDetails
            {
                AccountName = "AccountName",
                ProjectId = "ProjectID",
                ProjectName = "ProjectName"
            };
        }

        [TestMethod]
        public async Task NoBuildsStored_NoBuildsReturned_NoUpdate()
        {
            IList<Build> builds = new List<Build>();

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                AccountDetails = CreateAccountDetails()
            };

            IBuildService buildService = InitializeBuildService(builds);
            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });

            Assert.AreEqual(0, updates.Count);
            Assert.AreEqual(null, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(null, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual(null, subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task NoBuildsStored_OneInProgressNewReturned_OneInProgressUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    Id = "1234",
                    QueueTime = DateTime.Now
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            IBuildService buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });

            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(BuildStatus.InProgress, updates.First().Status);
            Assert.IsNull(updates.First().Result);
            Assert.AreEqual("First Definition", updates.First().Name);
            Assert.AreEqual(BuildStatus.InProgress, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(null, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task NoBuildsStored_OneCompletedNewReturned_OneCompleteUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Id = "1234",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.PartiallySucceeded,
                    QueueTime = DateTime.Now
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            IBuildService buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });

            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
            Assert.AreEqual(BuildResult.PartiallySucceeded, updates.First().Result);
            Assert.AreEqual("First Definition", updates.First().Name);
            Assert.AreEqual(BuildStatus.Completed, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.PartiallySucceeded, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task NoBuildsStored_OneCompletedOldReturned_NoUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Id = "1234",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.Canceled,
                    QueueTime = DateTime.Now.AddDays(-1)
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });

            Assert.AreEqual(0, updates.Count);
            Assert.AreEqual(BuildStatus.Completed, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Canceled, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task NoBuildsStored_OneNewInProgressOneNewCompleteReturned_NewestUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Id = "1234",
                    Status = BuildStatus.InProgress,
                    QueueTime = DateTime.Now
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Id = "1234",
                    Status = BuildStatus.Completed,
                    QueueTime = DateTime.Now.AddMinutes(-1)
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };
            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(BuildStatus.InProgress, updates.First().Status);
            Assert.IsNull(updates.First().Result);
            Assert.AreEqual("First Definition", updates.First().Name);
            Assert.AreEqual(BuildStatus.InProgress, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(null, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task NoBuildStored_OneNewInProgress_CurrentBuildRequestedForUpdated()
        {
            string requestedPerson = "Nicky";

            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    RequestedFor = requestedPerson
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                AccountDetails = CreateAccountDetails()
            };
            var buildService = InitializeBuildService(builds);

            await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });

            Assert.AreEqual(requestedPerson, subscribedBuild.CurrentBuildRequestedFor);
        }

        [TestMethod]
        public async Task NoBuildStored_OneNewInComplete_CurrentBuildAndLastCompletedRequestedForUpdated()
        {
            string requestedPerson = "Nicky";

            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.Failed,
                    RequestedFor = requestedPerson
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                AccountDetails = CreateAccountDetails()
            };
            var buildService = InitializeBuildService(builds);

            await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });

            Assert.AreEqual(requestedPerson, subscribedBuild.CurrentBuildRequestedFor);
            Assert.AreEqual(requestedPerson, subscribedBuild.LastCompletedBuildRequestedFor);
        }
        
        [TestMethod]
        public async Task InProgressStored_SameCompleteReturned_LastCompletedRequestedForUpdatedCurrentBuildRequestedSame()
        {
            string requestedPerson = "Nicky";

            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.Failed,
                    RequestedFor = requestedPerson
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                AccountDetails = CreateAccountDetails(),
                LastCompletedBuildRequestedFor = "Someone else",
                CurrentBuildStatus = BuildStatus.InProgress
            };
            var buildService = InitializeBuildService(builds);

            await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });

            Assert.AreEqual(requestedPerson, subscribedBuild.CurrentBuildRequestedFor);
            Assert.AreEqual(requestedPerson, subscribedBuild.LastCompletedBuildRequestedFor);
        }
        
        [TestMethod]
        public async Task InProgressStored_SameCompleteAndNewInProgressReturned_BothRequestedForUpdated()
        {
            string firstRequestedPerson = "Nicky";
            string secondRequestedPerson = "Another Nicky";

            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    RequestedFor = secondRequestedPerson,
                    Id = "5678"
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.PartiallySucceeded,
                    RequestedFor = firstRequestedPerson,
                    Id = "1234"
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                AccountDetails = CreateAccountDetails(),
                CurrentBuildStatus = BuildStatus.InProgress,
                LastCompletedBuildRequestedFor = "Someone",
                CurrentBuildRequestedFor = "Someone else",
                CurrentBuildId = "1234"
            };
            var buildService = InitializeBuildService(builds);

            await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });

            Assert.AreEqual(secondRequestedPerson, subscribedBuild.CurrentBuildRequestedFor);
            Assert.AreEqual(firstRequestedPerson, subscribedBuild.LastCompletedBuildRequestedFor);
        }

        public async Task CompleteStored_NewInProgressReturned_CurrentRequestedForUdatedOnly()
        {
            string firstRequestedPerson = "Nicky";
            string secondRequestedPerson = "Another Nicky";

            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    RequestedFor = secondRequestedPerson,
                    Id = "5678"
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.PartiallySucceeded,
                    RequestedFor = firstRequestedPerson,
                    Id = "1234"
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                AccountDetails = CreateAccountDetails(),
                CurrentBuildStatus = BuildStatus.Completed,
                LastCompletedBuildRequestedFor = firstRequestedPerson,
                CurrentBuildRequestedFor = firstRequestedPerson,
                CurrentBuildId = "1234"
            };
            var buildService = InitializeBuildService(builds);

            await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });

            Assert.AreEqual(secondRequestedPerson, subscribedBuild.CurrentBuildRequestedFor);
            Assert.AreEqual(firstRequestedPerson, subscribedBuild.LastCompletedBuildRequestedFor);
        }

        [TestMethod]
        public async Task InProgressStored_SameInProgressReturned_NoUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    Id = "1234"
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "1234",
                CurrentBuildStatus = BuildStatus.InProgress,
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(0, updates.Count);
            Assert.AreEqual(BuildStatus.InProgress, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(null, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task InProgressStored_SameCompleteSuccessReturned_OneCompleteSuccessUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Result = BuildResult.Succeeded,
                    Status = BuildStatus.Completed,
                    Id = "1234"
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "1234",
                CurrentBuildStatus = BuildStatus.InProgress,
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
            Assert.AreEqual(BuildResult.Succeeded, updates.First().Result);
            Assert.AreEqual("First Definition", updates.First().Name);
            Assert.AreEqual(BuildStatus.Completed, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Succeeded, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task InProgressStored_SameCompleteFailReturned_OneCompleteFailUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Result = BuildResult.Failed,
                    Status = BuildStatus.Completed,
                    Id = "1234"
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "1234",
                CurrentBuildStatus = BuildStatus.InProgress,
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
            Assert.AreEqual(BuildResult.Failed, updates.First().Result);
            Assert.AreEqual("First Definition", updates.First().Name);
            Assert.AreEqual(BuildStatus.Completed, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Failed, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task InProgressStored_SameInProgressOldCompleteReturned_NoUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    Id = "1234",
                    QueueTime = DateTime.Now
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.Completed,
                    Id = "4567",
                    QueueTime = DateTime.Now.AddMinutes(-1)
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "1234",
                CurrentBuildStatus = BuildStatus.InProgress,
                LastCompletedBuildResult = BuildResult.PartiallySucceeded,
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(0, updates.Count);
            Assert.AreEqual(BuildStatus.InProgress, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.PartiallySucceeded, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task InProgressStored_SameCompleteSuccessOldCompleteReturned_OneCompleteSuccessUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Result = BuildResult.Succeeded,
                    Status = BuildStatus.Completed,
                    Id = "1234",
                    QueueTime = DateTime.Now
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Result = BuildResult.Failed,
                    Status = BuildStatus.Completed,
                    Id = "4567",
                    QueueTime = DateTime.Now.AddMinutes(-1)
                }
            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "1234",
                CurrentBuildStatus = BuildStatus.InProgress,
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
            Assert.AreEqual(BuildResult.Succeeded, updates.First().Result);
            Assert.AreEqual("First Definition", updates.First().Name);
            Assert.AreEqual(BuildStatus.Completed, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Succeeded, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task InProgressStored_SameCompleteFailOldCompleteReturned_OneCompleteFailUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Result = BuildResult.Failed,
                    Status = BuildStatus.Completed,
                    Id = "1234",
                    QueueTime = DateTime.Now
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Result = BuildResult.Succeeded,
                    Status = BuildStatus.Completed,
                    Id = "4567",
                    QueueTime = DateTime.Now.AddMinutes(-1)
                }

            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "1234",
                CurrentBuildStatus = BuildStatus.InProgress,
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
            Assert.AreEqual(BuildResult.Failed, updates.First().Result);
            Assert.AreEqual("First Definition", updates.First().Name);
            Assert.AreEqual(BuildStatus.Completed, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Failed, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task CompleteStored_NewInProgressReturned_OneInProgressUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    Id = "1234",
                    QueueTime = DateTime.Now
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Result = BuildResult.Succeeded,
                    Status = BuildStatus.Completed,
                    Id = "4567",
                    QueueTime = DateTime.Now.AddMinutes(-1)
                }

            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "4567",
                CurrentBuildStatus = BuildStatus.Completed,
                LastCompletedBuildResult = BuildResult.Succeeded,
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(BuildStatus.InProgress, updates.First().Status);
            Assert.IsNull(updates.First().Result);
            Assert.AreEqual("First Definition", updates.First().Name);
            Assert.AreEqual(BuildStatus.InProgress, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Succeeded, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task InProgressStored_SameCompleteNewInProgressReturned_CompleteInProgressUpdates()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    Id = "1234",
                    QueueTime = DateTime.Now
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Result = BuildResult.Succeeded,
                    Status = BuildStatus.Completed,
                    Id = "4567",
                    QueueTime = DateTime.Now.AddMinutes(-1)
                }

            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "4567",
                CurrentBuildStatus = BuildStatus.InProgress,
                LastCompletedBuildResult = BuildResult.Failed,
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(2, updates.Count);
            Assert.IsTrue(updates.Any(u => u.Status == BuildStatus.Completed && u.Result == BuildResult.Succeeded && u.Name == "First Definition"));
            Assert.IsTrue(updates.Any(u => u.Status == BuildStatus.InProgress && u.Result == null && u.Name == "First Definition"));
            Assert.AreEqual(BuildStatus.InProgress, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Succeeded, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task NotStartedStored_SameInProgressReturned_InProgressUpdates()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    Id = "1234",
                    QueueTime = DateTime.Now
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.InProgress,
                    Id = "1234",
                    QueueTime = DateTime.Now.AddMinutes(-1)
                }

            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "1234",
                LastCompletedBuildResult = BuildResult.Failed,
                CurrentBuildStatus = BuildStatus.NotStarted,
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(BuildStatus.InProgress, updates.First().Status);
            Assert.IsNull(updates.First().Result);
            Assert.AreEqual("First Definition", updates.First().Name);
            Assert.AreEqual(BuildStatus.InProgress, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Failed, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task SuccessStored_TwoCompleteReturnedNotNew_NoUpdate()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.Succeeded,
                    Id = "1234",
                    QueueTime = DateTime.Now
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.Succeeded,
                    Id = "5678",
                    QueueTime = DateTime.Now.AddMinutes(-1)
                }

            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "1234",
                LastCompletedBuildResult = BuildResult.Succeeded,
                CurrentBuildStatus = BuildStatus.Completed,
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(0, updates.Count);
            Assert.AreEqual(BuildStatus.Completed, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Succeeded, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        [TestMethod]
        public async Task BuildStored_TwoNewBuildsReturned_BuildDefinitionUpdated()
        {
            IList<Build> builds = new List<Build>
            {
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.Succeeded,
                    Id = "1234",
                    QueueTime = DateTime.Now
                },
                new Build
                {
                    BuildDefinitionId = "1",
                    Status = BuildStatus.Completed,
                    Result = BuildResult.Succeeded,
                    Id = "5678",
                    QueueTime = DateTime.Now.AddMinutes(-1)
                }

            };

            SubscribedBuild subscribedBuild = new SubscribedBuild
            {
                BuildDefinitionId = "1",
                CurrentBuildId = "09876",
                LastCompletedBuildResult = BuildResult.Succeeded,
                CurrentBuildStatus = BuildStatus.Completed,
                Name = "First Definition",
                AccountDetails = CreateAccountDetails()
            };

            var buildService = InitializeBuildService(builds);

            IList<BuildUpdate> updates = await buildService.CheckForUpdatedBuilds(new List<SubscribedBuild> { subscribedBuild });


            Assert.AreEqual(BuildStatus.Completed, subscribedBuild.CurrentBuildStatus);
            Assert.AreEqual(BuildResult.Succeeded, subscribedBuild.LastCompletedBuildResult);
            Assert.AreEqual("1234", subscribedBuild.CurrentBuildId);
        }

        // todo test returned in the correct order
    }
}
