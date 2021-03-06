﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Client;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using StockServer.Conversations.StockStreamRequest;

namespace StockServerTest.Conversations
{
    /// <summary>
    /// Summary description for MessageFactoryTest
    /// </summary>
    [TestClass]
    public class StockStreamRequestTest
    {
        private Mock<StockHistoryResponseState> mock;

        public Conversation ConversationBuilder(Envelope env)
        {
            Conversation conv = null;

            switch (env.Contents)
            {
                case StockHistoryRequestMessage m:
                    conv = new StockHistoryResponseConversation(env);
                    mock = new Mock<StockHistoryResponseState>(env, conv) { CallBase = true };
                    conv.SetInitialState(mock.Object as StockHistoryResponseState);
                    break;
            }

            return conv;
        }

        //TODO: this will change when tcp is done
        [TestInitialize]
        public void TestInitialize()
        {
            ComService.AddUdpClient(Config.DEFAULT_UDP_CLIENT, 0);
            ConversationManager.Start(ConversationBuilder);
        }

        //TODO: this will change when tcp is done
        [TestCleanup]
        public void TestCleanup()
        {
            ConversationManager.Stop();
            ComService.RemoveClient(Config.DEFAULT_UDP_CLIENT);
        }

        [TestMethod]
        public void InvalidMessageResponderTest()
        {
            //Simulate remote application-level ids
            string incomingConversationID = "5-23";
            int remoteProcessId = 2;
            int remotePortfolioId = 3;

            //Create a fake incoming message to simulate an ack StockStreamRequest
            var message = MessageFactory.GetMessage<AckMessage>(remoteProcessId, remotePortfolioId);
            message.ConversationID = incomingConversationID;
            var messageEnvelope = new Envelope(message);

            //Handle "incoming" message
            var replyConversation = ComService.HandleIncomingMessage(messageEnvelope);

            //Verify conversation was NOT built from message
            Assert.IsNull(replyConversation);
            Assert.IsFalse(ConversationManager.ConversationExists(incomingConversationID));
        }

        [TestMethod]
        public void RequestSucceed()
        {
            string RequestConvId = "5-562";
            string RequestMessageId = "5-7-1654";
            string ClientIp = "192.168.1.31";
            int ClientPort = 5682;

            var RequestMessage = new StockHistoryRequestMessage()
            {
                ConversationID = RequestConvId,
                MessageID = RequestMessageId
            };

            Envelope Request = new TcpEnvelope(RequestMessage, ClientIp, ClientPort);

            var localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNull(localConv);
            Assert.IsNull(mock);

            var numClients = ClientManager.Clients.Count;
            
            ConversationManager.ProcessIncomingMessage(Request);

            localConv = ConversationManager.GetConversation(RequestConvId);

            Assert.IsNotNull(localConv);
            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);
        }

        [TestMethod]
        public void RequestSucceedAfterOneIncomingRetry()
        {
            string RequestConvId = "5-562";
            string RequestMessageId = "5-7-1654";
            string ClientIp = "192.168.1.31";
            int ClientPort = 5682;

            var RequestMessage = new StockHistoryRequestMessage()
            {
                ConversationID = RequestConvId,
                MessageID = RequestMessageId
            };
            Envelope Request = new TcpEnvelope(RequestMessage, ClientIp, ClientPort);

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
        }

    }
}
