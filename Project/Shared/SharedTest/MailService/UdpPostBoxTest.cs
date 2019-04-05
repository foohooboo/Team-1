using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.ComService;

namespace SharedTest.MailService
{
    [TestClass]
    public class UdpPostBoxTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var address = "127.0.0.1:1342";
            var udp = new UdpPostBox(address);

            Assert.AreEqual(address, udp.LocalEndPoint.ToString());
        }
    }
}
