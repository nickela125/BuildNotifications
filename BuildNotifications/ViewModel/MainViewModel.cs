using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
        private string _filterPropertyName;

        public MainViewModel(IAccountService accountService, IBuildService buildService, IMessenger messenger)
        {
            _accountService = accountService;
            _buildService = buildService;
            _messenger = messenger;

            CloseCommand = new RelayCommand<CancelEventArgs>(Close);
            ManageBuildsCommand = new RelayCommand(ManageBuilds);
            BuildsMenuItemCommand = new RelayCommand(BuildsMenuItem);
            ExitMenuItemCommand = new RelayCommand(ExitMenuItem);
            
            StatusFilterOptions = new List<string>
            {
                Constants.FilterByResult,
                Constants.FilterByStatus
            };

            SelectedFilterOption = StatusFilterOptions.First();
            _filterPropertyName = Constants.BuildResultPropertyName;

            SubscribedBuilds = new ObservableCollection<SubscribedBuild>(_buildService.GetSubscribedBuilds());
            _messenger.Register<SubscribedBuildsUpdate>(this, update =>
            {
                SubscribedBuilds = new ObservableCollection<SubscribedBuild>(((SubscribedBuildsUpdate)update).SubscribedBuilds);
                GroupedSubscribedBuilds = new ListCollectionView(SubscribedBuilds);
                GroupedSubscribedBuilds.GroupDescriptions.Add(new PropertyGroupDescription(_filterPropertyName));
                _icon.Icon = GetIconForBuilds();
            });

            GroupedSubscribedBuilds = new ListCollectionView(SubscribedBuilds);
            GroupedSubscribedBuilds.GroupDescriptions.Add(new PropertyGroupDescription(_filterPropertyName));


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
                    Placement = PlacementMode.AbsolutePoint,
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

        private ObservableCollection<SubscribedBuild> _subscribedBuilds; 
        public ObservableCollection<SubscribedBuild> SubscribedBuilds
        {
            get { return _subscribedBuilds; }
            set
            {
                Set(ref _subscribedBuilds, value);
                RaisePropertyChanged(() => SubscribedBuilds);
            }
        }

        private ListCollectionView _groupedSubscribedBuilds; 
        public ListCollectionView GroupedSubscribedBuilds
        {
            get { return _groupedSubscribedBuilds; }
            set { Set(ref _groupedSubscribedBuilds, value); }
        }

        private IList<string> _statusFilterOptions; 
        public IList<string> StatusFilterOptions
        {
            get { return _statusFilterOptions; }
            set { Set(ref _statusFilterOptions, value); }
        }

        private string _selectedFilterOption; 
        public string SelectedFilterOption
        {
            get { return _selectedFilterOption; }
            set
            {
                Set(ref _selectedFilterOption, value);

                _filterPropertyName = SelectedFilterOption == 
                    Constants.FilterByStatus ? 
                    Constants.BuildStatusPropertyName : 
                    Constants.BuildResultPropertyName;
                if (GroupedSubscribedBuilds != null && GroupedSubscribedBuilds.GroupDescriptions != null)
                {
                    GroupedSubscribedBuilds.GroupDescriptions.Clear();
                    GroupedSubscribedBuilds.GroupDescriptions.Add(new PropertyGroupDescription(_filterPropertyName));
                }
            }
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

        private async void CheckBuilds(object sender, EventArgs e)
        {
            IList<BuildUpdate> updates = await _buildService.CheckForUpdatedBuilds(SubscribedBuilds);
            if (updates.Any())
            {
                NotifyOfUpdates(updates);
                _icon.Icon = GetIconForBuilds();
            }
        }

        private void NotifyOfUpdates(IList<BuildUpdate> updates)
        {
            foreach (BuildUpdate vsoBuildUpdate in updates)
            {
                if (vsoBuildUpdate.Result == null)
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
                else
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
            List<BuildResult?> results = SubscribedBuilds.Select(sb => sb.LastCompletedBuildResult).ToList();

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