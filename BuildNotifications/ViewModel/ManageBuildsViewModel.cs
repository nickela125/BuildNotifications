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
        private IList<VsoAccount> _accounts;
        private bool _isUpdateEnabled;
        private bool _notifyOnStart;
        private bool _notifyOnFinish;

        public ManageBuildsViewModel(IAccountService accountService, IMessenger messenger)
        {
            _accountService = accountService;
            _messenger = messenger;

            Accounts = _accountService.GetAccounts();
            _messenger.Register<AccountsUpdate>(this, update =>
            {
                Accounts = _accountService.GetAccounts();
            });

            CloseDialogCommand = new RelayCommand(CloseDialog);
            UpdateAccountsCommand = new RelayCommand(UpdateAccounts);
            EditAccountCommand = new RelayCommand<VsoAccount>(EditAccount);
            RemoveAccountCommand = new RelayCommand<VsoAccount>(RemoveAccount);
            AddAccountCommand = new RelayCommand(AddAccount);

            IsUpdateEnabled = true;
            NotifyOnStart = _accountService.GetNotifyOnStart();
            NotifyOnFinish = _accountService.GetNotifyOnFinish();
        }

        #region Properties
        public RelayCommand CloseDialogCommand { get; }
        public RelayCommand UpdateAccountsCommand { get; }
        public RelayCommand<VsoAccount> EditAccountCommand { get; }
        public RelayCommand<VsoAccount> RemoveAccountCommand { get; }
        public RelayCommand AddAccountCommand { get; }

        public IList<VsoAccount> Accounts
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
            
            _accountService.SaveAccounts(Accounts);
            _accountService.SaveNotifyOptions(NotifyOnStart, NotifyOnFinish);
            CloseDialog();

             IsUpdateEnabled = true;
        }

        private void EditAccount(VsoAccount account)
        {
            ConfigureAccountWindow configureAccountWindow = new ConfigureAccountWindow
            {
                Top = Application.Current.MainWindow.Top + 100,
                Left = Application.Current.MainWindow.Left + 100
            };

            configureAccountWindow.ShowDialog();
        }

        private void RemoveAccount(VsoAccount account)
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
