using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.ComService;

namespace SharedTest.CommSystem
{
    [TestClass]
    public class UdpClientTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var address = "127.0.0.1:1342";
            var udp = new UdpClient(address);

            Assert.AreEqual(address, udp.LocalEndPoint.ToString());
        }
    }
}
