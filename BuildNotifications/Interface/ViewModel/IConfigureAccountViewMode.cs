using GalaSoft.MvvmLight.Command;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IConfigureAccountViewModel
    {
        RelayCommand CloseCommand { get; }
    }
}
