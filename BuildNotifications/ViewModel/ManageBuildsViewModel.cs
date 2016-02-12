using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using BuildNotifications.Model.Message;
using BuildNotifications.ViewModel.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace BuildNotifications.ViewModel
{
    public class ManageBuildsViewModel : ViewModelBase, IManageBuildsViewModel
    {
        private readonly IAccountService _accountService;
        private readonly IMessenger _messenger;
        private readonly IBuildService _buildService;
        private IList<Account> _accounts;
        private bool _isUpdateEnabled;
        private bool _notifyOnStart;
        private bool _notifyOnFinish;

        public ManageBuildsViewModel(IAccountService accountService, IMessenger messenger, IBuildService buildService)
        {
            _accountService = accountService;
            _messenger = messenger;
            _buildService = buildService;

            Accounts = _accountService.GetAccounts();
            _messenger.Register<AccountsUpdate>(this, update =>
            {
                Accounts = _accountService.GetAccounts();
            });

            CloseDialogCommand = new RelayCommand(CloseDialog);
            UpdateAccountsCommand = new RelayCommand(UpdateAccounts);
            EditAccountCommand = new RelayCommand<Account>(EditAccount);
            RemoveAccountCommand = new RelayCommand<Account>(RemoveAccount);
            RefreshAccountCommand = new RelayCommand<Account>(RefreshAccount);
            AddAccountCommand = new RelayCommand(AddAccount);

            IsUpdateEnabled = true;
            NotifyOnStart = _buildService.GetNotifyOnStart();
            NotifyOnFinish = _buildService.GetNotifyOnFinish();
        }

        #region Properties
        public RelayCommand CloseDialogCommand { get; }
        public RelayCommand UpdateAccountsCommand { get; }
        public RelayCommand<Account> EditAccountCommand { get; }
        public RelayCommand<Account> RemoveAccountCommand { get; }
        public RelayCommand<Account> RefreshAccountCommand { get; }
        public RelayCommand AddAccountCommand { get; }

        public IList<Account> Accounts
        {
            get { return _accounts; }
            set { Set(ref _accounts, value); }
        }

        public bool IsUpdateEnabled
        {
            get { return _isUpdateEnabled; }
            set { Set(ref _isUpdateEnabled, value); }
        }

        public bool NotifyOnStart
        {
            get { return _notifyOnStart; }
            set { Set(ref _notifyOnStart, value); }
        }

        public bool NotifyOnFinish
        {
            get { return _notifyOnFinish; }
            set { Set(ref _notifyOnFinish, value); }
        }

        #endregion


        #region Commands

        private void CloseDialog()
        {
            ApplicationHelper.CloseFrontWindow();
        }

        private void UpdateAccounts()
        {
            IsUpdateEnabled = false;
            
            _accountService.UpdateAccountSubsciptions(Accounts);
            _buildService.SaveNotifyOptions(NotifyOnStart, NotifyOnFinish);
            CloseDialog();

            IsUpdateEnabled = true;
        }

        private void EditAccount(Account account)
        {
            ConfigureAccountWindow configureAccountWindow = new ConfigureAccountWindow
            {
                Top = Application.Current.MainWindow.Top + 100,
                Left = Application.Current.MainWindow.Left + 100
            };

            configureAccountWindow.ShowDialog();
        }

        private void RemoveAccount(Account account)
        {
            MessageBoxResult confirmation = MessageBox.Show(
                $"Are you sure you want to remove the account '{account.Name}'?",
                "Confirm Remove Account",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                _accountService.RemoveAccount(account);
            }
        }

        private void RefreshAccount(Account account)
        {
            _accountService.UpdateAccount(account);
        }

        private void AddAccount()
        {
            ConfigureAccountWindow configureAccountWindow = new ConfigureAccountWindow
            {
                Top = Application.Current.MainWindow.Top + 100,
                Left = Application.Current.MainWindow.Left + 100
            };

            configureAccountWindow.ShowDialog();
        }

        #endregion
    }
}
