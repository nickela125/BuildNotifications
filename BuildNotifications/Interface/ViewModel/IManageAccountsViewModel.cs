using System.Collections.Generic;
using BuildNotifications.Model;
using GalaSoft.MvvmLight.Command;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IManageAccountsViewModel
    {
        IList<VsoAccount> Accounts { get; set; }
        RelayCommand CloseDialogCommand { get; }
        RelayCommand UpdateAccountsCommand { get; }
        bool IsUpdateEnabled { get; }
        bool NotifyOnStart { get; }
        bool NotifyOnFinish { get; }
    }
}
