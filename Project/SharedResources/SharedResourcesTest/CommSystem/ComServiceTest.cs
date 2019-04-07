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

            var a1_localPort = 354;
            var a1_id = "a1Id";

            Assert.IsFalse(ComService.HasClient());
            ComService.AddClient(a1_id, a1_localPort);
            Assert.IsTrue(ComService.HasClient());
            ComService.RemoveClient(a1_id);
        }

        [TestMethod]
        public void AccessClientTest()
        {

            var a1_localPort = 231;
            var a1_id = "a1Id";

            var a2_localPort = 241;
            var a2_id = "a2Id";

            var a3_localPort = 261;
            var a3_id = "a3Id";

            

            ComService.AddClient(a1_id, a1_localPort);
            ComService.AddClient(a2_id, a2_localPort);
            ComService.AddClient(a3_id, a3_localPort);

            var pb = ComService.GetClient(a2_id);
            Assert.AreEqual(a2_localPort, pb.getConnectedPort());

            ComService.RemoveClient(a1_id);
            ComService.RemoveClient(a2_id);
            ComService.RemoveClient(a3_id);
        }

        [TestMethod]
        public void RemoveClientTest()
        {
            var a1_localPort = 231;
            var a1_id = "a1Id";

            var a2_localPort = 241;
            var a2_id = "a2Id";

            ComService.AddClient(a1_id, a1_localPort);
            ComService.AddClient(a2_id, a2_localPort);

            Assert.IsNotNull(ComService.GetClient(a2_id));
            ComService.RemoveClient(a2_id);
            Assert.IsNull(ComService.GetClient(a2_id));

            ComService.RemoveClient(a1_id);
        }
    }
}