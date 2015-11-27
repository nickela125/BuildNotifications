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
    public class ConfigureAccountViewModel : ViewModelBase, IConfigureAccountViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ConfigureAccountViewModel()
        {
            CloseCommand = new RelayCommand(Close);
        }

        #region Properties

        public RelayCommand CloseCommand { get; }

        #endregion

        #region Commands


        private void Close()
        {
            CloseDialog();
        }

        #endregion
        
        private void CloseDialog()
        {
            int numberOfWindows = Application.Current.Windows.Count;
            Application.Current.Windows[numberOfWindows - 1].Close();
        }
    }
}