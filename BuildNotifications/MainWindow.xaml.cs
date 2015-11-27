using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BuildNotifications
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        // Don't close the app, hide to tray
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void TaskbarIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void MenuItem_ShowBuilds(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void MenuItem_Quit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
