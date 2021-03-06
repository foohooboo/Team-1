﻿using Broker;
using Broker.Conversations.TransactionRequest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;
using Shared.PortfolioResources;

namespace BrokerTest.Conversations
{
    [TestClass]
    public class TransactionTest
    {
        private Mock<RespondTransaction_InitialState> mock;

        public Conversation ConversationBuilder(Envelope env)
        {
            Conversation conv = null;

            switch (env.Contents)
            {
                case TransactionRequestMessage m:
                    conv = new RespondTransactionConversation(env);

                    //setup response message as mock
                    mock = new Mock<RespondTransaction_InitialState>(conv, env) { CallBase = true };
                    conv.SetInitialState(mock.Object as RespondTransaction_InitialState);

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
            string RequestConvId = "5-562";
            string ClientIp = "192.168.1.31";
            int ClientPort = 5682;
            int RequestQuanitity = 12;
            PortfolioManager.TryToCreate("TestRequestSucceed", "password", out Portfolio portfolio);

            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,1,2,3,100,5").Split(','), testStock);

            var RequestMessage = new TransactionRequestMessage(RequestQuanitity, vStock)
            {
                ConversationID = RequestConvId,
                PortfolioId = portfolio.PortfolioID
            };

            Envelope Request = new Envelope(RequestMessage, ClientIp, ClientPort);

            var localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNull(localConv);
            Assert.IsNull(mock);

            ConversationManager.ProcessIncomingMessage(Request);

            localConv = ConversationManager.GetConversation(RequestConvId);

            Assert.IsNotNull(localConv);
            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);

            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }

        [TestMethod]
        public void RequestSucceedAfterIncomingRetry()
        {
            string RequestConvId = "5-563";
            string ClientIp = "192.168.1.31";
            int ClientPort = 5682;
            int RequestQuanitity = 12;
            PortfolioManager.TryToCreate("TestRequestSucceedAfterRetry", "password", out Portfolio portfolio);

            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,1,2,3,100,5").Split(','), testStock);
            var RequestMessage = new TransactionRequestMessage(RequestQuanitity, vStock)
            {
                ConversationID = RequestConvId,
                PortfolioId = portfolio.PortfolioID
            };

            Envelope Request = new Envelope(RequestMessage, ClientIp, ClientPort);

            var localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNull(localConv);
            Assert.IsNull(mock);

            ConversationManager.ProcessIncomingMessage(Request);

            localConv = ConversationManager.GetConversation(RequestConvId);

            Assert.IsNotNull(localConv);
            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);

            ConversationManager.ProcessIncomingMessage(Request);

            localConv = ConversationManager.GetConversation(RequestConvId);

            Assert.IsNotNull(localConv);
            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Exactly(2));
            mock.Verify(state => state.HandleTimeout(), Times.Never);

            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }
    }
}