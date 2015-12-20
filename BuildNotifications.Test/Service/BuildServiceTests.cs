using System.Collections.Generic;
using BuildNotifications.Client;
using BuildNotifications.Interface.Client;
using BuildNotifications.Model;
using BuildNotifications.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildNotifications.Test.Service
{
    [TestClass]
    public class BuildServiceTests
    {
        private IVsoClient Initialize(IList<VsoBuild> builds)
        {
            return new VsoClient(new RestClient(), new Mapper());
        }

        [TestMethod]
        public void TestMethod1()
        {
            var buildService = new BuildService(Initialize(null));
        }
    }
}
