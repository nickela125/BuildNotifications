using System.ComponentModel;
using GalaSoft.MvvmLight.Command;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IMainViewModel
    {
        RelayCommand<CancelEventArgs> CloseCommand { get; }
        RelayCommand DoubleClickNotificationIconCommand { get; }
        RelayCommand BuildsMenuItemCommand { get; }
        RelayCommand ExitMenuItemCommand { get; }
    }
}
