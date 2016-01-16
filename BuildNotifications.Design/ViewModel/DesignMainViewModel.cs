using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                            ProjectId = "123456"
                        },
                        Name = "First Build",
                        LastCompletedBuildResult = BuildResult.Succeeded
                    },
                    new SubscribedBuild
                    {
                        AccountDetails = new AccountDetails
                        {
                            AccountName = "Nickys Account",
                            EncodedCredentials = "1234567",
                            ProjectId = "123456"
                        },
                        Name = "Second Build",
                        LastCompletedBuildResult = BuildResult.PartiallySucceeded
                    },
                    new SubscribedBuild
                    {
                        AccountDetails = new AccountDetails
                        {
                            AccountName = "Nickys Account",
                            EncodedCredentials = "1234567",
                            ProjectId = "123456"
                        },
                        Name = "Third Build",
                        LastCompletedBuildResult = BuildResult.Failed
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
                GroupedSubscribedBuildsInternal.GroupDescriptions.Add(new PropertyGroupDescription("LastCompletedBuildResult"));
                return GroupedSubscribedBuildsInternal;
            }
            set { }
        }
    }
}
