using GalaSoft.MvvmLight.Command;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IBuildListViewModel
    {
        RelayCommand ManageAccountsCommand { get; }
    }
}
