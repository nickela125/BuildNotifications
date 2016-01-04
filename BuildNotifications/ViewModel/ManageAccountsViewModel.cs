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
    public class ManageAccountsViewModel : ViewModelBase, IManageAccountsViewModel
    {
        private readonly IAccountService _accountService;
        private readonly IMessenger _messenger;
        private IList<VsoAccount> _accounts;
        private bool _isUpdateEnabled;
        private bool _notifyOnStart;
        private bool _notifyOnFinish;

        public ManageAccountsViewModel(IAccountService accountService, IMessenger messenger)
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
            AddAccountCommand = new RelayCommand(AddAccount);

            IsUpdateEnabled = true;
            NotifyOnStart = _accountService.GetNotifyOnStart();
            NotifyOnFinish = _accountService.GetNotifyOnFinish();
        }

        #region Properties
        public RelayCommand CloseDialogCommand { get; }
        public RelayCommand UpdateAccountsCommand { get; }
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
