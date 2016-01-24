using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace BuildNotifications.Design.ViewModel
{
    public class DesignMainViewModel : IMainViewModel
    {
        public RelayCommand<CancelEventArgs> CloseCommand { get; }
        public RelayCommand ManageBuildsCommand { get; }
        public ObservableCollection<SubscribedBuild> SubscribedBuilds {
            get
            {
                return new ObservableCollection<SubscribedBuild>
                {
                    new SubscribedBuild
                    {
                        AccountDetails = new AccountDetails
                        {
                            AccountName = "Nickys Account",
                            EncodedCredentials = "1234567",
                            ProjectId = "123456",
                            ProjectName = "My project"
                        },
                        Name = "First Build",
                        LastCompletedBuildResult = BuildResult.Succeeded,
                        LastCompletedBuildRequestedFor = "Me",
                        CurrentBuildRequestedFor = "You",
                        LastBuildResultChangeTime = DateTime.Now,
                        LastBuildStatusChangeTime = DateTime.Now
                    },
                    new SubscribedBuild
                    {
                        AccountDetails = new AccountDetails
                        {
                            AccountName = "Nickys Account",
                            EncodedCredentials = "1234567",
                            ProjectId = "123456",
                            ProjectName = "Oh hey another project"
                        },
                        Name = "Second Build",
                        LastCompletedBuildResult = BuildResult.PartiallySucceeded,
                        LastCompletedBuildRequestedFor = "A Fox",
                        CurrentBuildRequestedFor = "A Cat",
                        LastBuildResultChangeTime = DateTime.Now.AddMinutes(-200),
                        LastBuildStatusChangeTime = DateTime.Now.AddMinutes(-200)
                    },
                    new SubscribedBuild
                    {
                        AccountDetails = new AccountDetails
                        {
                            AccountName = "Nickys Account",
                            EncodedCredentials = "1234567",
                            ProjectId = "123456",
                            ProjectName = "A name for a project"
                        },
                        Name = "Third Build",
                        LastCompletedBuildResult = BuildResult.Failed,
                        LastCompletedBuildRequestedFor = "Nicky Crawford",
                        CurrentBuildRequestedFor = "Harry Potter",
                        LastBuildResultChangeTime = DateTime.Now.AddMinutes(200),
                        LastBuildStatusChangeTime = DateTime.Now.AddMinutes(200)
                    }
                };
            }
            set
            {}
        }

        public ListCollectionView GroupedSubscribedBuilds
        {
            get
            {
                var GroupedSubscribedBuildsInternal = new ListCollectionView(SubscribedBuilds);
                GroupedSubscribedBuildsInternal.GroupDescriptions.Add(new PropertyGroupDescription("CurrentBuildStatus"));
                return GroupedSubscribedBuildsInternal;
            }
            set { }
        }

        public IList<string> StatusFilterOptions => new List<string> { "Current Build Status" };
        public string SelectedFilterOption
        {
            get { return StatusFilterOptions.First(); }
            set {  }
        }

        public bool ShowingResults { get; set; }
    }
}
