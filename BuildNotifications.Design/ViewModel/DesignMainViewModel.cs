using System.Collections.Generic;
using System.ComponentModel;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace BuildNotifications.Design.ViewModel
{
    public class DesignMainViewModel : IMainViewModel
    {
        public RelayCommand<CancelEventArgs> CloseCommand { get; }
        public RelayCommand ManageBuildsCommand { get; }
        public IList<SubscribedBuild> SubscribedBuilds {
            get
            {
                return new List<SubscribedBuild>
                {
                    new SubscribedBuild
                    {
                        AccountDetails = new AccountDetails
                        {
                            AccountName = "Nickys Account",
                            EncodedCredentials = "1234567",
                            ProjectId = "123456"
                        },
                        DisplayName = "First Build",
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
                        DisplayName = "Second Build",
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
                        DisplayName = "Third Build",
                        LastCompletedBuildResult = BuildResult.Failed
                    }
                };
            }
            set
            {}
        }
    }
}
