using System.ComponentModel;
using System.Windows;
using BuildNotifications.Interface.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BuildNotifications.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            // Set up Commands
            CloseCommand = new RelayCommand<CancelEventArgs>(Close);
            DoubleClickNotificationIconCommand = new RelayCommand(DoubleClickNotificationIcon);
            BuildsMenuItemCommand = new RelayCommand(BuildsMenuItem);
            ExitMenuItemCommand = new RelayCommand(ExitMenuItem);
        }

        #region Properties

        public RelayCommand<CancelEventArgs> CloseCommand { get; }
        public RelayCommand DoubleClickNotificationIconCommand { get; }
        public RelayCommand BuildsMenuItemCommand { get; }
        public RelayCommand ExitMenuItemCommand { get; }

        #endregion

        #region Commands

        private void ExitMenuItem()
        {
            Application.Current.Shutdown();
        }

        private void BuildsMenuItem()
        {
            Application.Current.MainWindow.Show();
        }

        private void DoubleClickNotificationIcon()
        {
            
        }

        private void Close(CancelEventArgs eventArgs)
        {
            eventArgs.Cancel = true;
            Application.Current.MainWindow.Hide();
        }

        #endregion

    }
}