using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IConfigureAccountViewModel
    {
        string VsoAccount { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        RelayCommand CloseDialogCommand { get; }
        RelayCommand<PasswordBox> SaveAccountCommand { get; }
    }
}
