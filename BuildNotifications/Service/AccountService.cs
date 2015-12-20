using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Model;
using Newtonsoft.Json;

namespace BuildNotifications.Service
{
    public class AccountService : IAccountService
    {
        private readonly IVsoClient _vsoClient;

        public AccountService(IVsoClient vsoClient)
        {
            _vsoClient = vsoClient;
        }

        public async Task UpdateAccount(VsoAccount account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            // Get any updates to projects and builds
            account.Projects = await _vsoClient.GetProjects(account.Name, account.EncodedCredentials);
            foreach (VsoProject vsoProject in account.Projects)
            {
                vsoProject.Builds = await _vsoClient.GetBuildDefinitions(vsoProject, account.Name, account.EncodedCredentials);
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
            // todo notify of update
        }

        public void SaveAccounts(IList<VsoAccount> accounts)
        {
            string jsonString = JsonConvert.SerializeObject(accounts);
            Properties.Settings.Default[Constants.AccountsConfigurationName] = jsonString;
            Properties.Settings.Default.Save();
        }

        public bool GetNotifyOnStart()
        {
            return (bool)Properties.Settings.Default[Constants.NotifyOnStartConfigurationName];
        }

        public bool GetNotifyOnFinish()
        {
            return (bool)Properties.Settings.Default[Constants.NotifyOnFinishConfigurationName];
        }

        public void SaveNotifyOptions(bool? notifyOnStart, bool? notifyOnFinish)
        {
            if (notifyOnStart != null)
            {
                Properties.Settings.Default[Constants.NotifyOnStartConfigurationName] = notifyOnStart;
            }

            if (notifyOnFinish != null)
            {
                Properties.Settings.Default[Constants.NotifyOnFinishConfigurationName] = notifyOnFinish;
            }

            Properties.Settings.Default.Save();
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
