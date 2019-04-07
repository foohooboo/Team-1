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
            var localPort = 1342;
            var udp = new UdpClient(localPort);

            Assert.AreEqual(localPort, udp.getConnectedPort());
        }
    }
}
