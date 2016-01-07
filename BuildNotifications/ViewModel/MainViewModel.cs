using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Controls.Primitives;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Application = System.Windows.Application;
using System.Windows.Forms;
using BuildNotifications.Model.Message;
using GalaSoft.MvvmLight.Messaging;
using Hardcodet.Wpf.TaskbarNotification;
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
            ManageBuildsCommand = new RelayCommand(ManageBuilds);
            BuildsMenuItemCommand = new RelayCommand(BuildsMenuItem);
            ExitMenuItemCommand = new RelayCommand(ExitMenuItem);

            BuildAccounts = GetSubscribedBuilds();
            _messenger.Register<AccountsUpdate>(this, update =>
            {
                BuildAccounts = GetSubscribedBuilds();
                _icon.Icon = GetIconForBuilds();
            });
            // todo show correct icon

            _icon = new TaskbarIcon
            {
                Name = "NotifyIcon",
                Icon = GetIconForBuilds(),
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
                },
                DoubleClickCommand = BuildsMenuItemCommand,
                ToolTipText = "Build Notifications"
            };

            _icon.TrayBalloonTipClicked += (sender, args) =>
            {
                // todo need to get url here
            };
            
            InitTimer();
        }

        #region Properties

        public RelayCommand<CancelEventArgs> CloseCommand { get; }
        private RelayCommand BuildsMenuItemCommand { get; }
        private RelayCommand ExitMenuItemCommand { get; }
        public RelayCommand ManageBuildsCommand { get; }
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

        private void Close(CancelEventArgs eventArgs)
        {
            eventArgs.Cancel = true;
            Application.Current.MainWindow.Hide();
        }
        private void ManageBuilds()
        {
            ManageBuildsWindow manageBuildsWindow = new ManageBuildsWindow
            {
                Top = Application.Current.MainWindow.Top + 100,
                Left = Application.Current.MainWindow.Left + 100
            };

            manageBuildsWindow.ShowDialog();
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
                    _icon.Icon = GetIconForBuilds();
                }

                // TODO - don't need to update every time - only if something has changed
                _accountService.UpdateBuildStatus(
                    vsoBuildAccount.AccountDetails.AccountName,
                    vsoBuildAccount.AccountDetails.ProjectId,
                    vsoBuildAccount.BuildDefinitions.ToList());
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

        private Icon GetIconForBuilds()
        {
            List<BuildResult?> results = BuildAccounts.SelectMany(ba => ba.BuildDefinitions.Select(bd => bd.LastCompletedBuildResult)).ToList();

            if (results.Any(r => r.GetValueOrDefault(BuildResult.Succeeded) == BuildResult.Failed))
            {
                return new Icon("Icons/cross.ico");
            }
            if (results.Any(r => r.GetValueOrDefault(BuildResult.Succeeded) == BuildResult.PartiallySucceeded))
            {
                return new Icon("Icons/warning.ico");
            }
            if (results.Any(r => r.GetValueOrDefault(BuildResult.Succeeded) == BuildResult.Canceled))
            {
                return new Icon("Icons/info.ico");
            }
            if (results.All(r => r.GetValueOrDefault(BuildResult.Failed) == BuildResult.Succeeded))
            {
                return new Icon("Icons/tick.ico");
            }
            return new Icon("Icons/question.ico");
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