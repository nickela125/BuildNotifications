using System.Collections.Generic;
using System.ComponentModel;
using BuildNotifications.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IMainViewModel
    {
        RelayCommand<CancelEventArgs> CloseCommand { get; }
        RelayCommand DoubleClickNotificationIconCommand { get; }
        RelayCommand BuildsMenuItemCommand { get; }
        RelayCommand ExitMenuItemCommand { get; }
        RelayCommand ManageAccountsCommand { get; }
        IList<VsoSubscibedBuildList> BuildAccounts { get; set; }
    }
}
