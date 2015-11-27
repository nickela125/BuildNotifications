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
    public class BuildListViewModel : ViewModelBase, IBuildListViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public BuildListViewModel()
        {
            ConfigureAccountCommand = new RelayCommand(ConfigureAccount);
        }

        #region Properties
        public RelayCommand ConfigureAccountCommand { get; }

        #endregion

        #region Commands

        private void ConfigureAccount()
        {
            ConfigureAccountWindow configuruAccountWindow = new ConfigureAccountWindow
            {
                Top = Application.Current.MainWindow.Top + 100,
                Left = Application.Current.MainWindow.Left + 100
            };

            configuruAccountWindow.ShowDialog();
        }

        #endregion

    }
}