using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.ComService;

namespace SharedTest.CommSystem
{
    [TestClass]
    public class ComServiceTest
    {
        [TestMethod]
        public void AddClientTest()
        {
            var a1 = @"127.0.0.1:231";

            Assert.IsFalse(ComService.HasClient());
            ComService.AddClient(a1);
            Assert.IsTrue(ComService.HasClient());
            ComService.RemoveClient(a1);
        }

        [TestMethod]
        public void AccessClientTest()
        {
            var a1 = @"127.0.0.1:231";
            var a2 = @"127.0.0.1:241";
            var a3 = @"127.0.0.1:261";

            ComService.AddClient(a1);
            ComService.AddClient(a2);
            ComService.AddClient(a3);

            var pb = ComService.GetClient(a2);
            var addressParts = a2.Split(':');

            Assert.AreEqual(addressParts[0], pb.LocalEndPoint.Address.ToString());
            Assert.AreEqual(addressParts[1], pb.LocalEndPoint.Port.ToString());

            ComService.RemoveClient(a1);
            ComService.RemoveClient(a2);
            ComService.RemoveClient(a3);
        }

        [TestMethod]
        public void RemoveClientTest()
        {
            var a1 = @"127.0.0.1:231";
            var a2 = @"127.0.0.1:211";

            ComService.AddClient(a1);
            ComService.AddClient(a2);

            ComService.RemoveClient(a2);
            Assert.IsNull(ComService.GetClient(a2));

            ComService.RemoveClient(a1);
        }
    }
}