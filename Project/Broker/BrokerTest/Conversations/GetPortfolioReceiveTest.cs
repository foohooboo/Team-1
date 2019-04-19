using Broker;
using Broker.Conversations.CreatePortfolio;
using Broker.Conversations.GetPortfolio;
using Broker.Conversations.GetPortfolioResponse;
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
    public class GetPortfolioReceiveTest
    {
        private Mock<GetPortfolioReceiveState> mock;

        public Conversation ConversationBuilder(Envelope e)
        {
            Conversation conv = null;

            switch (e.Contents)
            {
                case GetPortfolioRequest m:
                    conv = new GetPortfoliolResponseConversation(m.ConversationID);

                    mock = new Mock<GetPortfolioReceiveState>(e, conv) { CallBase = true };
                    conv.SetInitialState(mock.Object as GetPortfolioReceiveState);
                    break;
            }

            return conv;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ComService.AddUdpClient(Config.DEFAULT_UDP_CLIENT, 0);
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

            PortfolioManager.TryToCreate(PortName, PortPass, out Portfolio preExistingPortfolio);

            var RequestMessage = new GetPortfolioRequest()
            {
                ConversationID = RequestConvId,
                MessageID = RequestConvId,
                Account = new Portfolio
                {
                    Username = PortName,
                    Password = PortPass
                }
            };

            Envelope IncomingRequest = new Envelope(RequestMessage, ClientIp, ClientPort);

            var localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNull(localConv);
            Assert.IsNull(mock);

            Assert.IsTrue(PortfolioManager.Portfolios.Count == 1);

            ConversationManager.ProcessIncomingMessage(IncomingRequest);

            localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNotNull(localConv.CurrentState.PreviousState.OutboundMessage.Contents as PortfolioUpdateMessage);
            Assert.IsNull(localConv.CurrentState.PreviousState.OutboundMessage.Contents as ErrorMessage);
            

            Assert.IsNotNull(localConv);
            
            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);
            
            Assert.IsTrue(PortfolioManager.TryToRemove(preExistingPortfolio.PortfolioID));
        }

        [TestMethod]
        public void RequestFailInvalidUsername()
        {
            string PortName = "IamAUsername";
            string PortPass = "IamAPassword";
            string RequestConvId = "5-567";
            string ClientIp = "192.168.1.75";
            int ClientPort = 5655;

            PortfolioManager.TryToCreate(PortName, PortPass, out Portfolio preExistingPortfolio);

            var RequestMessage = new GetPortfolioRequest()
            {
                ConversationID = RequestConvId,
                MessageID = RequestConvId,
                Account = new Portfolio
                {
                    Username = PortName +"makemeInvalid",
                    Password = PortPass
                }
            };

            Envelope IncomingRequest = new Envelope(RequestMessage, ClientIp, ClientPort);

            var localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNull(localConv);
            Assert.IsNull(mock);

            Assert.IsTrue(PortfolioManager.Portfolios.Count == 1);

            ConversationManager.ProcessIncomingMessage(IncomingRequest);

            localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNotNull(localConv.CurrentState.PreviousState.OutboundMessage.Contents as ErrorMessage);
            Assert.IsNull(localConv.CurrentState.PreviousState.OutboundMessage.Contents as PortfolioUpdateMessage);

            Assert.IsNotNull(localConv);

            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);

            Assert.IsTrue(PortfolioManager.TryToRemove(preExistingPortfolio.PortfolioID));
        }

        [TestMethod]
        public void RequestFailInvalidPassword()
        {
            string PortName = "IamAUsernameff";
            string PortPass = "IamAPasswordfff";
            string RequestConvId = "5-564";
            string ClientIp = "192.168.1.99";
            int ClientPort = 5655;

            PortfolioManager.TryToCreate(PortName, PortPass, out Portfolio preExistingPortfolio);

            var RequestMessage = new GetPortfolioRequest()
            {
                ConversationID = RequestConvId,
                MessageID = RequestConvId,
                Account = new Portfolio
                {
                    Username = PortName,
                    Password = PortPass + "makemeInvalid"
                }
            };

            Envelope IncomingRequest = new Envelope(RequestMessage, ClientIp, ClientPort);

            var localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNull(localConv);
            Assert.IsNull(mock);

            Assert.IsTrue(PortfolioManager.Portfolios.Count == 1);

            ConversationManager.ProcessIncomingMessage(IncomingRequest);

            localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNotNull(localConv.CurrentState.PreviousState.OutboundMessage.Contents as ErrorMessage);
            Assert.IsNull(localConv.CurrentState.PreviousState.OutboundMessage.Contents as PortfolioUpdateMessage);

            Assert.IsNotNull(localConv);

            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);

            Assert.IsTrue(PortfolioManager.TryToRemove(preExistingPortfolio.PortfolioID));
        }


    }
}