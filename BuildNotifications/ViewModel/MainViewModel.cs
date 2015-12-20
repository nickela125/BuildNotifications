using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Application = System.Windows.Application;

namespace BuildNotifications.ViewModel
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly IAccountService _accountService;
        private readonly IBuildService _buildService;
        private Timer _timer;

        public MainViewModel(IAccountService accountService, IBuildService buildService)
        {
            _accountService = accountService;
            _buildService = buildService;
            CloseCommand = new RelayCommand<CancelEventArgs>(Close);
            DoubleClickNotificationIconCommand = new RelayCommand(DoubleClickNotificationIcon);
            BuildsMenuItemCommand = new RelayCommand(BuildsMenuItem);
            ExitMenuItemCommand = new RelayCommand(ExitMenuItem);
            InitTimer();
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

        #region private methods
        
        private void InitTimer()
        {
            _timer = new Timer();
            _timer.Tick += new EventHandler(CheckBuilds);
            _timer.Interval = 10000; // todo - configurable?
            _timer.Start();
        }

        private void CheckBuilds(object sender, EventArgs e)
        {
            IList<VsoAccount> accounts = _accountService.GetAccounts(); // todo shouldn't do this every time - get notification if updates
            // TODO Bad bad O(n^3)
            foreach (VsoAccount account in accounts)
            {
                foreach (VsoProject project in account.Projects)
                {
                    IList<VsoBuildDefinition> buildDefinitions = new List<VsoBuildDefinition>();
                    foreach (VsoBuildDefinition build in project.Builds)
                    {
                        if (build.IsSelected)
                        {
                            buildDefinitions.Add(build);
                        }
                    }

                    if (buildDefinitions.Any())
                    {
                        _buildService.CheckForUpdatedBuilds(project, account.Name, account.EncodedCredentials, buildDefinitions);
                    }
                }
            }
        }

        #endregion

    }
}