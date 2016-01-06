using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;
using BuildNotifications.Model.Message;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace BuildNotifications.Service
{
    public class AccountService : IAccountService
    {
        private readonly IVsoClient _vsoClient;
        private readonly IMessenger _messenger;

        public AccountService(IVsoClient vsoClient, IMessenger messenger)
        {
            _vsoClient = vsoClient;
            _messenger = messenger;
        }

        public async Task UpdateAccount(VsoAccount account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            AccountDetails accountDetails = new AccountDetails
            {
                AccountName = account.Name,
                EncodedCredentials = account.EncodedCredentials
            };

            // Get any updates to projects and builds
            account.Projects = await _vsoClient.GetProjects(accountDetails);

            foreach (VsoProject vsoProject in account.Projects)
            {
                accountDetails.ProjectId = vsoProject.Id;
                vsoProject.Builds = await _vsoClient.GetBuildDefinitions(accountDetails);
                vsoProject.Builds.ToList().ForEach(b => b.IsSelected = false);
            }

            // Replace existing reference to account if there is one
            IList<VsoAccount> accounts = GetAccounts();
            VsoAccount existingAccount = accounts.SingleOrDefault(a => a.Name == account.Name);

            if (existingAccount == null)
            {
                accounts.Add(account);
            }
            else
            {
                TransferSubscriptions(existingAccount, account);
                int indexOfExistingAccount = accounts.IndexOf(existingAccount);
                accounts[indexOfExistingAccount] = account;
            }

            SaveAccounts(accounts);
           _messenger.Send(new AccountsUpdate());
        }

        public void SaveAccounts(IList<VsoAccount> accounts)
        {
            string jsonString = JsonConvert.SerializeObject(accounts);
            Properties.Settings.Default[Constants.AccountsConfigurationName] = jsonString;
            Properties.Settings.Default.Save();
            _messenger.Send(new AccountsUpdate());
        }

        public void RemoveAccount(VsoAccount account)
        {
            IList<VsoAccount> accounts = GetAccounts();
            VsoAccount accountToRemove = accounts.Single(a => a.Name == account.Name);
            int accountIndex = accounts.IndexOf(accountToRemove);
            accounts.RemoveAt(accountIndex);
            SaveAccounts(accounts);
        }

        public void UpdateBuildDefinitions(IList<VsoSubscibedBuildList> subscibedBuilds) // todo more efficient way?
        {
            IList<VsoAccount> accounts = GetAccounts();

            foreach (VsoAccount account in accounts)
            {
                foreach (VsoProject vsoProject in account.Projects)
                {
                    VsoSubscibedBuildList buildList =
                    subscibedBuilds.SingleOrDefault(sb => sb.AccountDetails.AccountName == account.Name && sb.AccountDetails.ProjectId == vsoProject.Id);

                    if (buildList != null)
                    {
                        foreach (VsoBuildDefinition vsoBuildDefinition in vsoProject.Builds)
                        {
                            int existingDefinitionIndex =
                                vsoProject.Builds.ToList().FindIndex(b => b.Id == vsoBuildDefinition.Id);
                            vsoProject.Builds[existingDefinitionIndex] = vsoBuildDefinition;
                        }
                        
                    }
                }
            }
        }

        public bool GetNotifyOnStart()
        {
            return (bool)Properties.Settings.Default[Constants.NotifyOnStartConfigurationName];
        }

        public bool GetNotifyOnFinish()
        {
            return (bool)Properties.Settings.Default[Constants.NotifyOnFinishConfigurationName];
        }

        public void SaveNotifyOptions(bool notifyOnStart, bool notifyOnFinish)
        {
            Properties.Settings.Default[Constants.NotifyOnStartConfigurationName] = notifyOnStart;
            Properties.Settings.Default[Constants.NotifyOnFinishConfigurationName] = notifyOnFinish;
            Properties.Settings.Default.Save();

            _messenger.Send<NotifyOptionsUpdate>(new NotifyOptionsUpdate
            {
                NotifyOnStart = notifyOnStart,
                NotifyOnFinish = notifyOnFinish
            });
        }

        public void UpdateBuildStatus(string accountName, string projectId, IList<VsoBuildDefinition> updatedDefinitions)
        {
            if (accountName == null)
            {
                throw new ArgumentNullException("accountName");
            }
            if (projectId == null)
            {
                throw new ArgumentNullException("projectId");
            }
            if (updatedDefinitions == null || !updatedDefinitions.Any())
            {
                return;
            }

            IList<VsoAccount> accounts = GetAccounts();

            VsoAccount account = accounts.Single(a => a.Name == accountName);
            VsoProject project = account.Projects.Single(p => p.Id == projectId);
            foreach (VsoBuildDefinition update in updatedDefinitions)
            {
                VsoBuildDefinition oldBuildDefinition = project.Builds.Single(b => b.Id == update.Id);
                int buildDefinitionIndex = project.Builds.IndexOf(oldBuildDefinition);
                project.Builds[buildDefinitionIndex] = update;
            }

            SaveAccounts(accounts);
        }

        public IList<VsoAccount> GetAccounts()
        {
            string jsonString = (string)Properties.Settings.Default[Constants.AccountsConfigurationName];
            
            return JsonConvert.DeserializeObject<List<VsoAccount>>(jsonString);
        }

        private void TransferSubscriptions(VsoAccount oldAccount, VsoAccount newAccount)
        {
            // TODO
        }
    }
}
