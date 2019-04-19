using System;
using System.Threading;
using Client.Conversations;
using Client.Conversations.GetPortfolio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;

namespace ClientTest.Conversations
{
    [TestClass]
    public class GetPortfolioTest
    {

        [TestInitialize]
        public void TestInitialize()
        {
            ComService.AddUdpClient(Config.DEFAULT_UDP_CLIENT,0);
            ConversationManager.Start(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ConversationManager.Stop();
            ComService.RemoveClient(Config.DEFAULT_UDP_CLIENT);
        }

        [TestMethod]
        public void Succeed()
        {
            int processIdId = 42;
            var username = "myUsername";
            var password = "myPassword";
                                             
            var conv = new GetPortfolioRequestConversation(processIdId);

            //setup response message and mock
            var mock = new Mock<GetPortfolioRequestState>(username, password, null, conv) { CallBase = true };
            mock.Setup(st => st.Send())//Pretend message is sent and response comes back...
                .Callback(()=> {
                var responseMessage = new PortfolioUpdateMessage() { ConversationID = conv.Id, MessageID = "responseMessageID1234" };
                var responseEnv = new Envelope(responseMessage);
                ConversationManager.ProcessIncomingMessage(responseEnv); 
            }).CallBase().Verifiable();
            
            //execute test
            conv.SetInitialState(mock.Object as GetPortfolioRequestState);

            Assert.IsTrue(conv.CurrentState is GetPortfolioRequestState);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);

            ConversationManager.AddConversation(conv);

            Assert.IsFalse(conv.CurrentState is GetPortfolioRequestState);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
        }
    }
}
