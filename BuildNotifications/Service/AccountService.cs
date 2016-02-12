using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Common;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Model;
using BuildNotifications.Model.Message;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace BuildNotifications.Service
{
    public class AccountService : IAccountService
    {
        private readonly IVsoClient _vsoClient;
        private readonly IMessenger _messenger;
        private readonly ISettingsProvider _settingsProvider;

        public AccountService(IVsoClient vsoClient, IMessenger messenger, ISettingsProvider settingsProvider)
        {
            _vsoClient = vsoClient;
            _messenger = messenger;
            _settingsProvider = settingsProvider;
        }

        public async Task UpdateAccount(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            IList<Account> existingAccounts = GetAccounts();
            
            AccountDetails accountDetails = new AccountDetails
            {
                AccountName = account.Name,
                EncodedCredentials = account.EncodedCredentials
            };

            // Get any updates to projects and builds
            account.Projects = await _vsoClient.GetProjects(accountDetails);

            foreach (Project vsoProject in account.Projects)
            {
                accountDetails.ProjectId = vsoProject.Id;
                vsoProject.Builds = await _vsoClient.GetBuildDefinitions(accountDetails);
                vsoProject.Builds.ToList().ForEach(b => b.IsSelected = false);
            }

            // Replace existing reference to account if there is one
            Account existingAccount = existingAccounts.SingleOrDefault(a => a.Name == account.Name);

            if (existingAccount == null)
            {
                existingAccounts.Add(account);
            }
            else
            {
                TransferSubscriptions(existingAccount, account); // todo - be careful that all data is transferred + new builds / projects included
                int indexOfExistingAccount = existingAccounts.IndexOf(existingAccount);
                existingAccounts[indexOfExistingAccount] = account;
            }

            SaveAccounts(existingAccounts);
        }

        public void UpdateAccountSubsciptions(IList<Account> updatedAccounts)
        {
            SaveAccounts(updatedAccounts);
            _messenger.Send(new AccountSubscriptionUpdate { Accounts = updatedAccounts });
        }

        public void RemoveAccount(Account account)
        {
            IList<Account> accounts = GetAccounts();
            Account accountToRemove = accounts.Single(a => a.Name == account.Name);
            int accountIndex = accounts.IndexOf(accountToRemove);
            accounts.RemoveAt(accountIndex);
            SaveAccounts(accounts);
            _messenger.Send(new AccountSubscriptionUpdate { Accounts = accounts });
        }

        public void SaveAccounts(IList<Account> accounts)
        {
            string jsonString = JsonConvert.SerializeObject(accounts);
            _settingsProvider.SaveSetting(Constants.AccountsConfigurationName, jsonString);
            _messenger.Send(new AccountsUpdate {Accounts = accounts});
        }

        public IList<Account> GetAccounts()
        {
            string jsonString = (string)_settingsProvider.GetSetting(Constants.AccountsConfigurationName);
            return JsonConvert.DeserializeObject<List<Account>>(jsonString);
        }

        private void TransferSubscriptions(Account oldAccount, Account newAccount)
        {
            newAccount.IsSelected = oldAccount.IsSelected;

            foreach (Project newProject in newAccount.Projects)
            {
                Project oldProject = oldAccount.Projects.SingleOrDefault(p => p.Id == newProject.Id);

                if (oldProject != null)
                {
                    newProject.IsSelected = oldProject.IsSelected;

                    foreach (BuildDefinition newBuildDefinition in newProject.Builds)
                    {
                        BuildDefinition oldBuildDefinition =
                            oldProject.Builds.SingleOrDefault(b => b.Id == newBuildDefinition.Id);

                        if (oldBuildDefinition != null)
                        {
                            newBuildDefinition.IsSelected = oldBuildDefinition.IsSelected;
                        }
                    }
                }
            }
        }
    }
}
