using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Interface.Client;
using BuildNotifications.Model;
using BuildNotifications.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BuildNotifications.Test.Service
{
    [TestClass]
    public class BuildServiceTests
    {
        // todo rewrite tests
        //private IVsoClient Initialize(IList<VsoBuild> builds)
        //{
        //    var mockVsoClient = new Mock<IVsoClient>();
        //    mockVsoClient.Setup(
        //        x =>
        //            x.GetBuilds(It.IsAny<AccountDetails>(), It.IsAny<IList<string>>()))
        //        .Returns(Task.FromResult(builds));

        //    return mockVsoClient.Object;
        //}

        //[TestMethod]
        //public void NoBuildsStored_NoBuildsReturned_NoUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>();

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition { Id = "1" };

        //    var buildService = new BuildService(Initialize(builds));
        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(), 
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(0, updates.Count);
        //    Assert.AreEqual(null, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(null, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual(null, buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void NoBuildsStored_OneInProgressNewReturned_OneInProgressUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.InProgress,
        //            Id = "1234",
        //            QueueTime = DateTime.Now
        //        }
        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition { Id = "1", DisplayName = "First Definition"};
        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(), 
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(1, updates.Count);
        //    Assert.AreEqual(BuildStatus.InProgress, updates.First().Status);
        //    Assert.IsNull(updates.First().Result);
        //    Assert.AreEqual("First Definition", updates.First().DisplayName);
        //    Assert.AreEqual(BuildStatus.InProgress, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(null, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void NoBuildsStored_OneCompletedNewReturned_OneCompleteUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Id = "1234",
        //            Status = BuildStatus.Completed,
        //            Result = BuildResult.PartiallySucceeded,
        //            QueueTime = DateTime.Now
        //        }
        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition { Id = "1", DisplayName = "First Definition" };
        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(), 
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(1, updates.Count);
        //    Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
        //    Assert.AreEqual(BuildResult.PartiallySucceeded, updates.First().Result);
        //    Assert.AreEqual("First Definition", updates.First().DisplayName);
        //    Assert.AreEqual(BuildStatus.Completed, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.PartiallySucceeded, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void NoBuildsStored_OneCompletedOldReturned_NoUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Id = "1234",
        //            Status = BuildStatus.Completed,
        //            Result = BuildResult.Canceled,
        //            QueueTime = DateTime.Now.AddDays(-1)
        //        }
        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition { Id = "1" };
        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(0, updates.Count);
        //    Assert.AreEqual(BuildStatus.Completed, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Canceled, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void NoBuildsStored_OneNewInProgressOneNewCompleteReturned_NewestUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Id = "1234",
        //            Status = BuildStatus.InProgress,
        //            QueueTime = DateTime.Now
        //        },
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Id = "1234",
        //            Status = BuildStatus.Completed,
        //            QueueTime = DateTime.Now.AddMinutes(-1)
        //        }
        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition { Id = "1", DisplayName = "First Definition" };
        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(1, updates.Count);
        //    Assert.AreEqual(BuildStatus.InProgress, updates.First().Status);
        //    Assert.IsNull(updates.First().Result);
        //    Assert.AreEqual("First Definition", updates.First().DisplayName);
        //    Assert.AreEqual(BuildStatus.InProgress, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(null, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void InProgressStored_SameInProgressReturned_NoUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.InProgress,
        //            Id = "1234"
        //        }
        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1", CurrentBuildId = "1234", CurrentBuildStatus = BuildStatus.InProgress
        //    };
            
        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(0, updates.Count);
        //    Assert.AreEqual(BuildStatus.InProgress, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(null, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void InProgressStored_SameCompleteSuccessReturned_OneCompleteSuccessUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Result = BuildResult.Succeeded,
        //            Status = BuildStatus.Completed,
        //            Id = "1234"
        //        }
        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1", CurrentBuildId = "1234", CurrentBuildStatus = BuildStatus.InProgress,
        //        DisplayName = "First Definition"
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(1, updates.Count);
        //    Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
        //    Assert.AreEqual(BuildResult.Succeeded, updates.First().Result);
        //    Assert.AreEqual("First Definition", updates.First().DisplayName);
        //    Assert.AreEqual(BuildStatus.Completed, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Succeeded, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void InProgressStored_SameCompleteFailReturned_OneCompleteFailUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Result = BuildResult.Failed,
        //            Status = BuildStatus.Completed,
        //            Id = "1234"
        //        }
        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1", CurrentBuildId = "1234", CurrentBuildStatus = BuildStatus.InProgress,
        //        DisplayName = "First Definition"
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(1, updates.Count);
        //    Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
        //    Assert.AreEqual(BuildResult.Failed, updates.First().Result);
        //    Assert.AreEqual("First Definition", updates.First().DisplayName);
        //    Assert.AreEqual(BuildStatus.Completed, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Failed, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void InProgressStored_SameInProgressOldCompleteReturned_NoUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.InProgress,
        //            Id = "1234",
        //            QueueTime = DateTime.Now
        //        },
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.Completed,
        //            Id = "4567",
        //            QueueTime = DateTime.Now.AddMinutes(-1)
        //        }
        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1",
        //        CurrentBuildId = "1234",
        //        CurrentBuildStatus = BuildStatus.InProgress,
        //        LastCompletedBuildResult = BuildResult.PartiallySucceeded
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(0, updates.Count);
        //    Assert.AreEqual(BuildStatus.InProgress, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.PartiallySucceeded, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void InProgressStored_SameCompleteSuccessOldCompleteReturned_OneCompleteSuccessUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Result = BuildResult.Succeeded,
        //            Status = BuildStatus.Completed,
        //            Id = "1234",
        //            QueueTime = DateTime.Now
        //        },
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Result = BuildResult.Failed,
        //            Status = BuildStatus.Completed,
        //            Id = "4567",
        //            QueueTime = DateTime.Now.AddMinutes(-1)
        //        }
        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1", CurrentBuildId = "1234", CurrentBuildStatus = BuildStatus.InProgress,
        //        DisplayName = "First Definition"
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(1, updates.Count);
        //    Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
        //    Assert.AreEqual(BuildResult.Succeeded, updates.First().Result);
        //    Assert.AreEqual("First Definition", updates.First().DisplayName);
        //    Assert.AreEqual(BuildStatus.Completed, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Succeeded, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void InProgressStored_SameCompleteFailOldCompleteReturned_OneCompleteFailUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Result = BuildResult.Failed,
        //            Status = BuildStatus.Completed,
        //            Id = "1234",
        //            QueueTime = DateTime.Now
        //        },
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Result = BuildResult.Succeeded,
        //            Status = BuildStatus.Completed,
        //            Id = "4567",
        //            QueueTime = DateTime.Now.AddMinutes(-1)
        //        }

        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1", CurrentBuildId = "1234", CurrentBuildStatus = BuildStatus.InProgress,
        //        DisplayName = "First Definition"
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(1, updates.Count);
        //    Assert.AreEqual(BuildStatus.Completed, updates.First().Status);
        //    Assert.AreEqual(BuildResult.Failed, updates.First().Result);
        //    Assert.AreEqual("First Definition", updates.First().DisplayName);
        //    Assert.AreEqual(BuildStatus.Completed, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Failed, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void CompleteStored_NewInProgressReturned_OneInProgressUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.InProgress,
        //            Id = "1234",
        //            QueueTime = DateTime.Now
        //        },
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Result = BuildResult.Succeeded,
        //            Status = BuildStatus.Completed,
        //            Id = "4567",
        //            QueueTime = DateTime.Now.AddMinutes(-1)
        //        }

        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1",
        //        CurrentBuildId = "4567",
        //        CurrentBuildStatus = BuildStatus.Completed,
        //        LastCompletedBuildResult = BuildResult.Succeeded,
        //        DisplayName = "First Definition"
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(1, updates.Count);
        //    Assert.AreEqual(BuildStatus.InProgress, updates.First().Status);
        //    Assert.IsNull(updates.First().Result);
        //    Assert.AreEqual("First Definition", updates.First().DisplayName);
        //    Assert.AreEqual(BuildStatus.InProgress, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Succeeded, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void InProgressStored_SameCompleteNewInProgressReturned_CompleteInProgressUpdates()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.InProgress,
        //            Id = "1234",
        //            QueueTime = DateTime.Now
        //        },
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Result = BuildResult.Succeeded,
        //            Status = BuildStatus.Completed,
        //            Id = "4567",
        //            QueueTime = DateTime.Now.AddMinutes(-1)
        //        }

        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1",
        //        CurrentBuildId = "4567",
        //        CurrentBuildStatus = BuildStatus.InProgress,
        //        LastCompletedBuildResult = BuildResult.Failed,
        //        DisplayName = "First Definition"
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(2, updates.Count);
        //    Assert.IsTrue(updates.Any(u => u.Status == BuildStatus.Completed && u.Result == BuildResult.Succeeded && u.DisplayName == "First Definition"));
        //    Assert.IsTrue(updates.Any(u => u.Status == BuildStatus.InProgress && u.Result == null && u.DisplayName == "First Definition"));
        //    Assert.AreEqual(BuildStatus.InProgress, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Succeeded, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void NotStartedStored_SameInProgressReturned_InProgressUpdates()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.InProgress,
        //            Id = "1234",
        //            QueueTime = DateTime.Now
        //        },
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.InProgress,
        //            Id = "1234",
        //            QueueTime = DateTime.Now.AddMinutes(-1)
        //        }

        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1",
        //        CurrentBuildId = "1234",
        //        LastCompletedBuildResult = BuildResult.Failed,
        //        CurrentBuildStatus = BuildStatus.NotStarted,
        //        DisplayName = "First Definition"
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(1, updates.Count);
        //    Assert.AreEqual(BuildStatus.InProgress, updates.First().Status);
        //    Assert.IsNull(updates.First().Result);
        //    Assert.AreEqual("First Definition", updates.First().DisplayName);
        //    Assert.AreEqual(BuildStatus.InProgress, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Failed, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void SuccessStored_TwoCompleteReturnedNotNew_NoUpdate()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.Completed,
        //            Result = BuildResult.Succeeded,
        //            Id = "1234",
        //            QueueTime = DateTime.Now
        //        },
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.Completed,
        //            Result = BuildResult.Succeeded,
        //            Id = "5678",
        //            QueueTime = DateTime.Now.AddMinutes(-1)
        //        }

        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1",
        //        CurrentBuildId = "1234",
        //        LastCompletedBuildResult = BuildResult.Succeeded,
        //        CurrentBuildStatus = BuildStatus.Completed,
        //        DisplayName = "First Definition"
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;

        //    Assert.AreEqual(0, updates.Count);
        //    Assert.AreEqual(BuildStatus.Completed, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Succeeded, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        //[TestMethod]
        //public void BuildStored_TwoNewBuildsReturned_BuildDefinitionUpdated()
        //{
        //    IList<VsoBuild> builds = new List<VsoBuild>
        //    {
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.Completed,
        //            Result = BuildResult.Succeeded,
        //            Id = "1234",
        //            QueueTime = DateTime.Now
        //        },
        //        new VsoBuild
        //        {
        //            BuildDefinitionId = "1",
        //            Status = BuildStatus.Completed,
        //            Result = BuildResult.Succeeded,
        //            Id = "5678",
        //            QueueTime = DateTime.Now.AddMinutes(-1)
        //        }

        //    };

        //    VsoBuildDefinition buildDefinition = new VsoBuildDefinition
        //    {
        //        Id = "1",
        //        CurrentBuildId = "09876",
        //        LastCompletedBuildResult = BuildResult.Succeeded,
        //        CurrentBuildStatus = BuildStatus.Completed,
        //        DisplayName = "First Definition"
        //    };

        //    var buildService = new BuildService(Initialize(builds));

        //    IList<VsoBuildUpdate> updates = buildService.CheckForUpdatedBuilds(new AccountDetails(),
        //        new List<VsoBuildDefinition> { buildDefinition }).Result;
            
        //    Assert.AreEqual(BuildStatus.Completed, buildDefinition.CurrentBuildStatus);
        //    Assert.AreEqual(BuildResult.Succeeded, buildDefinition.LastCompletedBuildResult);
        //    Assert.AreEqual("1234", buildDefinition.CurrentBuildId);
        //}

        // todo test returned in the correct order
    }
}
