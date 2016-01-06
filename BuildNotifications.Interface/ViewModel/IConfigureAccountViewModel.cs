using System.Windows.Controls;
using GalaSoft.MvvmLight.CommandWpf;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IConfigureAccountViewModel
    {
        string VsoAccount { get; set; }
        string Username { get; set; }
        bool IsUpdateEnabled { get; set; }
        RelayCommand CloseDialogCommand { get; }
        RelayCommand<PasswordBox> SaveAccountCommand { get; }
    }
}
