using System;
using System.Threading;
using Client.Conversations;
using Client.Conversations.StockHistory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;

namespace ClientTest.Conversations
{
    [TestClass]
    public class StockHistoryTest
    {

        [TestInitialize]
        public void TestInitialize()
        {
            ConversationManager.Start(null);
            PostOffice.AddBox("0.0.0.0:0");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            PostOffice.RemoveBox("0.0.0.0:0");
        }

        [TestMethod]
        public void SucessfulRequest()
        {
            var stockStreamConv = new StockHistoryRequestConversation();
            stockStreamConv.SetInitialState(new StockHistoryRequestState(stockStreamConv));
            ConversationManager.AddConversation(stockStreamConv);
            string conversationId = stockStreamConv.Id;

            //Verify conversation exists in Conversation Manager
            Assert.IsTrue(ConversationManager.ConversationExists(conversationId));

            //Create fake response message and process it
            var stockStreamResponse = new Envelope(new StockStreamResponseMessage() { MessageID = "123-abc" });
            stockStreamResponse.Contents.ConversationID = stockStreamConv.Id;
            ConversationManager.ProcessIncomingMessage(stockStreamResponse);

            //Conversation over but we hold onto the done state for a little
            //in case we need to handle a retry. Ensure it has not yet been
            //removed from Conversation Manager
            Assert.IsTrue(ConversationManager.ConversationExists(conversationId));

            var retrycount = Config.GetInt(Config.DEFAULT_RETRY_COUNT);
            var timeout = Config.GetInt(Config.DEFAULT_TIMEOUT);
            Thread.Sleep((int)((retrycount*2) * timeout * 1.5));

            //Conversation should have cleaned itself up now...
            Assert.IsFalse(ConversationManager.ConversationExists(conversationId));
        }

        [TestMethod]
        public void RequestSingleTimeoutThenSucceed()
        {
            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

            var conv = new StockHistoryRequestConversation();
            int requests = 0;

            //setup mock with response message
            var mock = new Mock<StockHistoryRequestState>(conv) { CallBase = true };
            mock.Setup(st => st.Send())
                .Callback(() =>
                {
                    //Pretend message is sent and response comes back, but only after second request...
                    if (++requests > 1)
                    {
                        var responseMessage = new StockStreamResponseMessage()
                        {
                            ConversationID = conv.Id,
                            MessageID = "345-234-56"
                        };
                        var responseEnv = new Envelope(responseMessage);
                        ConversationManager.ProcessIncomingMessage(responseEnv);
                    }
                });

            conv.SetInitialState(mock.Object as StockHistoryRequestState);

            Assert.IsTrue(conv.CurrentState is StockHistoryRequestState);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);
            Assert.IsFalse(ConversationManager.ConversationExists(conv.Id));

            ConversationManager.AddConversation(conv);

            Assert.IsTrue(conv.CurrentState is StockHistoryRequestState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);
            Assert.IsTrue(ConversationManager.ConversationExists(conv.Id));

            Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * Config.GetInt(Config.DEFAULT_RETRY_COUNT) * 2.1));

            Assert.IsFalse(conv.CurrentState is StockHistoryRequestState);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Exactly(3));
            mock.Verify(state => state.HandleTimeout(), Times.Exactly(3));
            Assert.IsTrue(ConversationManager.ConversationExists(conv.Id));
        }

        [TestMethod]
        public void RequestTimeout()
        {
            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

            var conv = new StockHistoryRequestConversation();
            int requests = 0;

            //setup mock with response message
            var mock = new Mock<StockHistoryRequestState>(conv) { CallBase = true };
            mock.Setup(st => st.Send())
                .Callback(() =>
                {
                    //Pretend message is sent and response comes back, but only after second request...
                    if (++requests > 1)
                    {
                        var responseMessage = new StockStreamResponseMessage()
                        {
                            ConversationID = conv.Id,
                            MessageID = "345-234-56"
                        };
                        var responseEnv = new Envelope(responseMessage);
                        ConversationManager.ProcessIncomingMessage(responseEnv);
                    }
                });

            conv.SetInitialState(mock.Object as StockHistoryRequestState);

            Assert.IsTrue(conv.CurrentState is StockHistoryRequestState);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);
            Assert.IsFalse(ConversationManager.ConversationExists(conv.Id));

            ConversationManager.AddConversation(conv);

            Assert.IsTrue(conv.CurrentState is StockHistoryRequestState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);
            Assert.IsTrue(ConversationManager.ConversationExists(conv.Id));

            Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * Config.GetInt(Config.DEFAULT_RETRY_COUNT) * 2.1));

            Assert.IsFalse(conv.CurrentState is StockHistoryRequestState);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Exactly(3));
            mock.Verify(state => state.HandleTimeout(), Times.Exactly(3));
            Assert.IsTrue(ConversationManager.ConversationExists(conv.Id));
        }
    }
}
