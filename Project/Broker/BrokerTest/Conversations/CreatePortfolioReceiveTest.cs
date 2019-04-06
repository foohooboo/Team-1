using Broker;
using Broker.Conversations.CreatePortfolio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.PortfolioResources;
using System.Linq;

namespace BrokerTest.Conversations
{
    [TestClass]
    public class CreatePortfolioReceiveTest
    {
        private Mock<CreatePortfolioReceiveState> mock;

        public Conversation ConversationBuilder(Envelope e)
        {
            Conversation conv = null;

            switch (e.Contents)
            {
                case CreatePortfolioRequestMessage m:
                    conv = new CreatePortfoliolResponseConversation(m.ConversationID);

                    mock = new Mock<CreatePortfolioReceiveState>(e, conv) { CallBase = true };
                    conv.SetInitialState(mock.Object as CreatePortfolioReceiveState);
                    break;
            }

            return conv;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ComService.AddClient(Config.DEFAULT_UDP_CLIENT, 0);
            ConversationManager.Start(ConversationBuilder);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ConversationManager.Stop();
            ComService.RemoveClient(Config.DEFAULT_UDP_CLIENT);
        }

        [TestMethod]
        public void RequestSucceed()
        {
            string PortName = "IamAUsername";
            string PortPass = "IamAPassword";
            string RequestConvId = "5-567";
            string ClientIp = "192.168.1.75";
            int ClientPort = 5655;
            
            var RequestMessage = new CreatePortfolioRequestMessage()
            {
                ConversationID = RequestConvId,
                MessageID = RequestConvId,
                Account = new Portfolio
                {
                    Username = PortName,
                    Password = PortPass
                },
                ConfirmPassword = PortPass
            };

            Envelope IncomingRequest = new Envelope(RequestMessage, ClientIp, ClientPort);

            var localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNull(localConv);
            Assert.IsNull(mock);

            Assert.IsTrue(PortfolioManager.Portfolios.Count == 0);

            ConversationManager.ProcessIncomingMessage(IncomingRequest);

            Assert.IsTrue(PortfolioManager.Portfolios.Count == 1);
            Portfolio localPortfolio = PortfolioManager.Portfolios.First().Value;
            Assert.AreEqual(PortName, localPortfolio.Username);
            Assert.AreEqual(PortPass, localPortfolio.Password);

            localConv = ConversationManager.GetConversation(RequestConvId);
            
            Assert.IsNotNull(localConv);
            Assert.IsNotNull(localConv.CurrentState.PreviousState.OutboundMessage.Contents as PortfolioUpdateMessage);

            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);

            Assert.IsTrue(PortfolioManager.TryToRemove(localPortfolio.PortfolioID));
        }

        [TestMethod]
        public void RequestFailAlreadyExists()
        {
            string PortName = "IamAUsername";
            string PortPass = "IamAPassword";
            string RequestConvId = "5-567";
            string ClientIp = "192.168.1.75";
            int ClientPort = 5655;

            PortfolioManager.TryToCreate(PortName, PortPass, out Portfolio preExistingPortfolio);
            PortfolioManager.ReleaseLock(preExistingPortfolio);

            var RequestMessage = new CreatePortfolioRequestMessage()
            {
                ConversationID = RequestConvId,
                MessageID = RequestConvId,
                Account = new Portfolio
                {
                    Username = PortName,
                    Password = PortPass
                },
                ConfirmPassword = PortPass
            };

            Envelope IncomingRequest = new Envelope(RequestMessage, ClientIp, ClientPort);

            var localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNull(localConv);
            Assert.IsNull(mock);

            Assert.IsTrue(PortfolioManager.Portfolios.Count == 1);

            ConversationManager.ProcessIncomingMessage(IncomingRequest);

            localConv = ConversationManager.GetConversation(RequestConvId);
            
            Assert.IsNotNull(localConv);
            Assert.IsNotNull(localConv.CurrentState.PreviousState.OutboundMessage.Contents as ErrorMessage);
            Assert.IsNull(localConv.CurrentState.PreviousState.OutboundMessage.Contents as PortfolioUpdateMessage);

            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);
            
            Assert.IsTrue(PortfolioManager.TryToRemove(preExistingPortfolio.PortfolioID));
        }


    }
}