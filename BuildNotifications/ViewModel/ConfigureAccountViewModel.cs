using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BuildNotifications.ViewModel
{
    public class ConfigureAccountViewModel : ViewModelBase, IConfigureAccountViewModel
    {
        private readonly IVsoClient _vsoClient;
        private readonly IAccountService _accountService;
        private string _vsoAccount;
        private string _username;
        private string _password;

        public ConfigureAccountViewModel(IVsoClient vsoClient, IAccountService accountService)
        {
            _vsoClient = vsoClient;
            _accountService = accountService;
            CloseDialogCommand = new RelayCommand(CloseDialog);
            SaveAccountCommand = new RelayCommand<PasswordBox>(SaveAccount);
        }

        #region Properties

        public RelayCommand CloseDialogCommand { get; }
        public RelayCommand<PasswordBox> SaveAccountCommand { get; }

        public string VsoAccount
        {
            get { return _vsoAccount; }
            set { Set(ref _vsoAccount, value); }
        }

        public string Username
        {
            get { return _username; }
            set { Set(ref _username, value); }
        }

        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
        }

        #endregion

        #region Commands

        private async void SaveAccount(PasswordBox passwordBox)
        {
            var account = new VsoAccount
            {
                Name = VsoAccount,
                Username = Username,
                EncodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + passwordBox.Password))
            };

            try
            {
                account.Projects = await _vsoClient.GetProjects(account);
                _accountService.UpdateAccount(account);
            }
            catch (Exception)
            {
                // TODO show error dialog
            }
        }

        private void CloseDialog()
        {
            CloseFrontWindow();
        }

        #endregion
        
        private void CloseFrontWindow()
        {
            int numberOfWindows = Application.Current.Windows.Count;
            Window currentWindow = Application.Current.Windows[numberOfWindows - 1];
            currentWindow?.Close();
        }
    }
}