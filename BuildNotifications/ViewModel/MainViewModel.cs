using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Controls.Primitives;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Hardcodet.Wpf.TaskbarNotification;
using Application = System.Windows.Application;
using System.Windows.Forms;
using BuildNotifications.Model.Message;
using GalaSoft.MvvmLight.Messaging;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;

namespace BuildNotifications.ViewModel
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly IAccountService _accountService;
        private readonly IBuildService _buildService;
        private readonly IMessenger _messenger;
        private Timer _timer;
        private TaskbarIcon _icon;
        private bool _notifyOnBuildStart;
        private bool _notifyOnBuildFinish;

        public MainViewModel(IAccountService accountService, IBuildService buildService, IMessenger messenger)
        {
            _accountService = accountService;
            _buildService = buildService;
            _messenger = messenger;
            _notifyOnBuildStart = _accountService.GetNotifyOnStart();
            _notifyOnBuildFinish = _accountService.GetNotifyOnFinish();

            _messenger.Register<NotifyOptionsUpdate>(this, update =>
            {
                _notifyOnBuildStart = ((NotifyOptionsUpdate) update).NotifyOnStart;
                _notifyOnBuildFinish = ((NotifyOptionsUpdate) update).NotifyOnFinish;
            });

            CloseCommand = new RelayCommand<CancelEventArgs>(Close);
            DoubleClickNotificationIconCommand = new RelayCommand(DoubleClickNotificationIcon);
            BuildsMenuItemCommand = new RelayCommand(BuildsMenuItem);
            ExitMenuItemCommand = new RelayCommand(ExitMenuItem);
            ManageAccountsCommand = new RelayCommand(ManageAccounts);

            BuildAccounts = GetSubscribedBuilds();
            _messenger.Register<AccountsUpdate>(this, update =>
            {
                BuildAccounts = GetSubscribedBuilds();
            });
            // todo show correct icon

            _icon = new TaskbarIcon
            {
                Name = "NotifyIcon",
                Icon = GetIcon(null),
                ToolTip = "Build Notifications",
                MenuActivation = PopupActivationMode.RightClick,
                ContextMenu = new ContextMenu
                {
                    Items =
                    {
                        new MenuItem { Header = "Builds...", Command = BuildsMenuItemCommand },
                        new MenuItem { Header = "Exit", Command = ExitMenuItemCommand }
                    },
                    Placement = PlacementMode.AbsolutePoint, //todo get this in the right position
                    HorizontalOffset = 0,
                    VerticalOffset = 0
                }
            };
            
            InitTimer();
        }

        #region Properties

        public RelayCommand<CancelEventArgs> CloseCommand { get; }
        public RelayCommand DoubleClickNotificationIconCommand { get; }
        public RelayCommand BuildsMenuItemCommand { get; }
        public RelayCommand ExitMenuItemCommand { get; }
        public RelayCommand ManageAccountsCommand { get; }
        private IList<VsoSubscibedBuildList> _buildAccounts; 
        public IList<VsoSubscibedBuildList> BuildAccounts
        {
            get { return _buildAccounts; }
            set { Set(ref _buildAccounts, value); }
        }

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
        private void ManageAccounts()
        {
            ManageAccountsWindow manageAccountsWindow = new ManageAccountsWindow
            {
                Top = Application.Current.MainWindow.Top + 100,
                Left = Application.Current.MainWindow.Left + 100
            };

            manageAccountsWindow.ShowDialog();
        }

        #endregion

        #region private methods

        private void InitTimer()
        {
            _timer = new Timer();
            _timer.Tick += CheckBuilds;
            _timer.Interval = 10000;
            _timer.Start();
        }

        private IList<VsoSubscibedBuildList> GetSubscribedBuilds()
        {
            IList<VsoAccount> accounts = _accountService.GetAccounts();

            return (from account in accounts
                    where account.IsSelected == null || account.IsSelected.Value
                    from project in account.Projects
                    where project.IsSelected == null || project.IsSelected.Value
                    let definitions = project.Builds.Where(b => b.IsSelected != null && (bool)b.IsSelected).ToList()
                    where definitions.Any()
                    select new VsoSubscibedBuildList
                    {
                        BuildDefinitions = definitions,
                        AccountDetails = new AccountDetails
                        {
                            AccountName = account.Name,
                            EncodedCredentials = account.EncodedCredentials,
                            ProjectId = project.Id
                        }
                    }).ToList();
        }

        private async void CheckBuilds(object sender, EventArgs e)
        {
            foreach (VsoSubscibedBuildList vsoBuildAccount in BuildAccounts)
            {
                IList<VsoBuildUpdate> updates = await _buildService.CheckForUpdatedBuilds(
                    vsoBuildAccount.AccountDetails, vsoBuildAccount.BuildDefinitions);
                if (updates.Any())
                {
                    NotifyOfUpdates(updates);
                    _accountService.UpdateBuildStatus(
                        vsoBuildAccount.AccountDetails.AccountName,
                        vsoBuildAccount.AccountDetails.ProjectId,
                        vsoBuildAccount.BuildDefinitions.Where(bd => updates.Any(u => u.Id == bd.Id)).ToList());
                }
            }
        }

        private void NotifyOfUpdates(IList<VsoBuildUpdate> updates)
        {
            foreach (VsoBuildUpdate vsoBuildUpdate in updates)
            {
                if (vsoBuildUpdate.Result == null && _notifyOnBuildStart)
                {
                    // Only one we care about for now
                    if (vsoBuildUpdate.Status == BuildStatus.InProgress)
                    {
                        _icon.ShowBalloonTip(
                            $"{vsoBuildUpdate.Name} Started", 
                            $"Requested For: {vsoBuildUpdate.RequestedFor}", 
                            BalloonIcon.Info);
                    }
                }
                else if (_notifyOnBuildFinish)
                {
                    switch (vsoBuildUpdate.Result)
                    {
                        case BuildResult.Succeeded:
                            _icon.ShowBalloonTip(
                                $"{vsoBuildUpdate.Name} Succeeded",
                                $"Requested For: {vsoBuildUpdate.RequestedFor}", 
                                BalloonIcon.Info);
                            break;
                        case BuildResult.PartiallySucceeded:
                            _icon.ShowBalloonTip(
                                $"{vsoBuildUpdate.Name} Partially Succeeded",
                                $"Requested For: {vsoBuildUpdate.RequestedFor}",
                                BalloonIcon.Warning);
                            break;
                        case BuildResult.Failed:
                            _icon.ShowBalloonTip(
                                $"{vsoBuildUpdate.Name} Failed",
                                $"Requested For: {vsoBuildUpdate.RequestedFor}",
                                BalloonIcon.Error);
                            break;
                        case BuildResult.Canceled:
                            _icon.ShowBalloonTip(
                                $"{vsoBuildUpdate.Name} Cancelled",
                                $"Requested For: {vsoBuildUpdate.RequestedFor}",
                                BalloonIcon.Info);
                            break;
                    }
                }
            }
        }

        private Icon GetIcon(BuildResult? result)
        {
            switch (result)
            {
                case BuildResult.Succeeded:
                    return new Icon("Icons/tick.ico");
                case BuildResult.PartiallySucceeded:
                    return new Icon("Icons/warning.ico");
                case BuildResult.Failed:
                    return new Icon("Icons/cross.ico");
                case BuildResult.Canceled:
                    return new Icon("Icons/info.ico");
                default:
                    return new Icon("Icons/question.ico");
            }
        }

        #endregion

    }
}