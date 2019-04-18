using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;

namespace SharedTest.Configuration
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void ClientConfigTest()
        {
            //Assert.IsTrue(Config.GetString(Config.GetClientProcessNumber()).Equals("100"));
            //Assert.AreEqual(Config.GetClientProcessNumber(), 100);
        }

        [TestMethod]
        public void BrokerConfigTest()
        {
            Assert.IsTrue(Config.GetString(Config.BROKER_IP).Equals("127.0.0.1"));
            Assert.IsTrue(Config.GetString(Config.BROKER_PORT).Equals("5200"));
            Assert.IsTrue(Config.GetString(Config.BROKER_PROCESS_NUM).Equals("200"));
        }

        [TestMethod]
        public void StockServerConfigTest()
        {
            Assert.IsTrue(Config.GetString(Config.STOCK_SERVER_IP).Equals("127.0.0.1"));
            Assert.IsTrue(Config.GetString(Config.STOCK_SERVER_PORT).Equals("5300"));
            Assert.IsTrue(Config.GetString(Config.STOCK_SERVER_PROCESS_NUM).Equals("300"));
        }
    }
}