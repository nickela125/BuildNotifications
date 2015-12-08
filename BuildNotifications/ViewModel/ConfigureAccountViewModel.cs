using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Model;
using BuildNotifications.ViewModel.Helpers;
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
        private bool _isUpdateEnabled;

        public ConfigureAccountViewModel(IVsoClient vsoClient, IAccountService accountService)
        {
            _vsoClient = vsoClient;
            _accountService = accountService;
            CloseDialogCommand = new RelayCommand(CloseDialog);
            SaveAccountCommand = new RelayCommand<PasswordBox>(SaveAccount);
            IsUpdateEnabled = true;
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

        public bool IsUpdateEnabled
        {
            get { return _isUpdateEnabled; }
            set { Set(ref _isUpdateEnabled, value); }
        }

        #endregion

        #region Commands

        private async void SaveAccount(PasswordBox passwordBox)
        {
            IsUpdateEnabled = false;

            var account = new VsoAccount
            {
                Name = VsoAccount,
                Username = Username,
                EncodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + passwordBox.Password))
            };

            try
            {
                await _accountService.UpdateAccount(account);
                CloseDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Could not connect to your account with the credentials provided. Please try again.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            IsUpdateEnabled = true;
        }

        private void CloseDialog()
        {
            ApplicationHelper.CloseFrontWindow();
        }

        #endregion
    }
}