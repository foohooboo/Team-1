using System.Collections.Generic;
using System.Net;
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

            Assert.IsTrue(ClientManager.TryToAdd(3, endpoint));
        }

        [TestMethod]
        [DoNotParallelize]
        public void TryToAddFailTest()
        {
            var endpoint = EndPointParser.Parse("127.0.0.1:3456");

            Assert.IsFalse(ClientManager.TryToAdd(3, endpoint));
        }

        [TestMethod]
        [DoNotParallelize]
        public void TryToRemoveSucessTest()
        {
            var clientCount = ClientManager.Clients.Count;

            Assert.IsTrue(ClientManager.TryToRemove(3));
            Assert.AreEqual(clientCount - 1, ClientManager.Clients.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void TryToRemoveFailTest()
        {
            Assert.IsFalse(ClientManager.TryToRemove(5));
        }

        [TestMethod]
        [DoNotParallelize]
        public void UpdateClientsEmptyTest()
        {
            Assert.AreEqual(ClientManager.UpdateClients(null), 0);
        }

        [TestMethod]
        [DoNotParallelize]
        public void UpdateClientsSuccessTest()
        {
            var endPoints = new List<IPEndPoint>
            {
                EndPointParser.Parse("127.0.0.1:1234"),
                EndPointParser.Parse("127.0.0.1:5678"),
                EndPointParser.Parse("127.0.0.1:9012"),
                EndPointParser.Parse("127.0.0.1:2345")
            };

            int id = 0;
            foreach (var ep in endPoints)
            {
                ClientManager.TryToAdd(id, ep);
                id++;
            }

            Assert.AreEqual(ClientManager.UpdateClients(null), 4);
        }
    }
}
