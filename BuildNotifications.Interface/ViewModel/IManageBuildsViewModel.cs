using System.Collections.Generic;
using BuildNotifications.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IManageBuildsViewModel
    {
        IList<Account> Accounts { get; set; }
        RelayCommand CloseDialogCommand { get; }
        RelayCommand UpdateAccountsCommand { get; }
        RelayCommand<Account> EditAccountCommand { get; }
        RelayCommand<Account> RemoveAccountCommand { get; }
        RelayCommand AddAccountCommand { get; }
        bool IsUpdateEnabled { get; }
        bool NotifyOnStart { get; }
        bool NotifyOnFinish { get; }
    }
}
