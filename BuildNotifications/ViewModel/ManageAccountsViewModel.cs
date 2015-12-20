﻿using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using BuildNotifications.ViewModel.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;

namespace BuildNotifications.ViewModel
{
    public class ManageAccountsViewModel : ViewModelBase, IManageAccountsViewModel
    {
        private readonly IAccountService _accountService;
        private IList<VsoAccount> _accounts;
        private bool _isUpdateEnabled;
        private bool _notifyOnStart;
        private bool _notifyOnFinish;

        public ManageAccountsViewModel(IAccountService accountService)
        {
            _accountService = accountService;

            Accounts = _accountService.GetAccounts();

            CloseDialogCommand = new RelayCommand(CloseDialog);
            UpdateAccountsCommand = new RelayCommand(UpdateAccounts);

            IsUpdateEnabled = true;
            NotifyOnStart = _accountService.GetNotifyOnStart();
            NotifyOnFinish = _accountService.GetNotifyOnFinish();
        }

        #region Properties
        public RelayCommand CloseDialogCommand { get; }
        public RelayCommand UpdateAccountsCommand { get; }

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

        #endregion
    }
}