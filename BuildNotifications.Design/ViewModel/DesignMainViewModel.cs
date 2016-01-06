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
        public IList<VsoSubscibedBuildList> BuildAccounts {
            get
            {
                return new List<VsoSubscibedBuildList>
                {
                    new VsoSubscibedBuildList
                    {
                        AccountDetails = new AccountDetails
                        {
                            AccountName = "Nickys Account",
                            EncodedCredentials = "1234567",
                            ProjectId = "123456"
                        },
                        BuildDefinitions = new List<VsoBuildDefinition>
                        {
                            new VsoBuildDefinition
                            {
                                Name = "First Build",
                                LastCompletedBuildResult = BuildResult.Succeeded
                            },
                            new VsoBuildDefinition
                            {
                                Name = "Second Build",
                                LastCompletedBuildResult = BuildResult.PartiallySucceeded
                            },
                            new VsoBuildDefinition
                            {
                                Name = "Second Build",
                                LastCompletedBuildResult = BuildResult.Failed
                            },
                        }
                    }
                };
            }
            set
            {
            }
        }
    }
}
