using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BuildNotifications
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private EventLog _log;
        public App()
        {
            _log = new EventLog();
            _log.Source = "Build Notifications";
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Application.Current.DispatcherUnhandledException += HandleException;
            base.OnStartup(e);
        }

        private void HandleException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _log.WriteEntry(e.Exception.Message, EventLogEntryType.Error);
            e.Handled = true;
        }
    }
}
