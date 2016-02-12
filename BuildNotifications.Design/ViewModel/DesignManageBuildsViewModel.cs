using System.Collections.Generic;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace BuildNotifications.Design.ViewModel
{
    public class DesignManageBuildsViewModel : IManageBuildsViewModel
    {
        public IList<Account> Accounts
        {
            get
            {
                return new List<Account>
                {
                    new Account
                    {
                        Name = "Account Name",
                        Projects = new List<Project>
                        {
                            new Project
                            {
                                Name = "Project Name",
                                Builds = new List<BuildDefinition>
                                {
                                    new BuildDefinition
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
        public RelayCommand<Account> EditAccountCommand { get; }
        public RelayCommand<Account> RemoveAccountCommand { get; }
        public RelayCommand<Account> RefreshAccountCommand { get; }
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
