using BuildNotifications.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Service;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildNotifications.Test.Service
{
    [TestClass]
    public class AccountServiceTests
    {
        private IAccountService _accountService;

        public void Initialize()
        {
            _accountService = new AccountService(new VsoClient(new RestClient(), new Mapper()), new Messenger());
        }

        [TestMethod]
        public void NoAccounts_UpdateOneAccount_OneAccountNoSubscriptions()
        {

        }

        // TODO testing
    }
}
