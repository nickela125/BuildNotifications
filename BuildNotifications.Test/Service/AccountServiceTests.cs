using BuildNotifications.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildNotifications.Test.Service
{
    [TestClass]
    public class UnitTest1
    {
        private IAccountService _accountService;
        [TestInitialize]
        public void Initialize()
        {
            _accountService = new AccountService(new VsoClient(new RestClient(), new Mapper()));
        }

        [TestMethod]
        public void NoAccounts_UpdateOneAccount_OneAccountNoSubscriptions()
        {

        }
    }
}
