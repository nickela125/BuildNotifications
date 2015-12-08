using System.Windows;

namespace BuildNotifications.ViewModel.Helpers
{
    public static class ApplicationHelper
    {
        public static void CloseFrontWindow()
        {
            int numberOfWindows = Application.Current.Windows.Count;
            Window currentWindow = Application.Current.Windows[numberOfWindows - 1];
            currentWindow?.Close();
        }
    }
}
