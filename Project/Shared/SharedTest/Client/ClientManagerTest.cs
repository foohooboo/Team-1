using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Client;
using Shared.Comms.MailService;

namespace SharedTest.Cleint
{
    [TestClass]
    public class ClientManagerTest
    {
        [TestMethod]
        [DoNotParallelize]
        public void TryToAddSuccessTest()
        {
            var endpoint = EndPointParser.Parse("127.0.0.1:3456");

            Assert.IsTrue(ClientManager.TryToAdd(endpoint));
        }

        [TestMethod]
        [DoNotParallelize]
        public void TryToAddFailTest()
        {
            var endpoint = EndPointParser.Parse("127.0.0.1:3456");

            Assert.IsFalse(ClientManager.TryToAdd(endpoint));
        }

        [TestMethod]
        [DoNotParallelize]
        public void TryToRemoveSucessTest()
        {
            var endpoint = EndPointParser.Parse("127.0.0.1:3456");
            var clientCount = ClientManager.Clients.Count;

            Assert.IsTrue(ClientManager.TryToRemove(endpoint));
            Assert.AreEqual(clientCount - 1, ClientManager.Clients.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void TryToRemoveFailTest()
        {
            var endpoint = EndPointParser.Parse("127.0.0.1:3456");

            Assert.IsFalse(ClientManager.TryToRemove(endpoint));
        }
    }
}
