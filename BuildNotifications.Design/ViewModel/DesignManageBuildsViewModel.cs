using System.Collections.Generic;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace BuildNotifications.Design.ViewModel
{
    public class DesignManageBuildsViewModel : IManageBuildsViewModel
    {
        public IList<VsoAccount> Accounts
        {
            get
            {
                return new List<VsoAccount>
                {
                    new VsoAccount
                    {
                        Name = "Account Name",
                        Projects = new List<VsoProject>
                        {
                            new VsoProject
                            {
                                Name = "Project Name",
                                Builds = new List<VsoBuildDefinition>
                                {
                                    new VsoBuildDefinition
                                    {
                                        Name = "Build Name"
                                    }
                                }
                            }
                        }
                    }
                };
            }
            set { }
        }
        public RelayCommand CloseDialogCommand { get; }
        public RelayCommand UpdateAccountsCommand { get; }
        public RelayCommand<VsoAccount> EditAccountCommand { get; }
        public RelayCommand<VsoAccount> RemoveAccountCommand { get; }
        public RelayCommand AddAccountCommand { get; }

        public bool IsUpdateEnabled
        {
            get { return true; }
            set { }
        }
        public bool NotifyOnStart
        {
            get { return false; }
            set { }
        }
        public bool NotifyOnFinish
        {
            get { return true; }
            set { }
        }
    }
}
