using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.Conversations.StockStreamRequest.Initiator;
using Shared.MarketStructures;
using System.Threading;

namespace SharedTest.Conversations
{


    [TestClass]
    public class Test_ConvI_StockStreamRequest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ConversationManager.Start(null);
            ComService.AddClient("0.0.0.0:0");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ComService.RemoveClient("0.0.0.0:0");
        }

        [TestMethod]
        public void SucessfulStockStreamRequestTest()
        {
            //Simulate application-level ids
            int processId = 1;

            //Create a new StockStreamRequestConv_Initor conversation
            var stockStreamConv = new ConvI_StockStreamRequest(processId);
            stockStreamConv.SetInitialState(new InitialState_ConvI_StockStreamRequest(stockStreamConv));
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
            Thread.Sleep(retrycount * timeout * 2);

            //Conversation should have cleaned itself up now...
            Assert.IsFalse(ConversationManager.ConversationExists(conversationId));
        }

        [TestMethod]
        public void RequestSingleTimeoutThenSucceed()
        {
            int processId = 1;
            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

            var conv = new ConvI_StockStreamRequest(processId);
            int requests = 0;

            //setup mock with response message
            var mock = new Mock<InitialState_ConvI_StockStreamRequest>(conv) { CallBase = true };
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

            conv.SetInitialState(mock.Object as InitialState_ConvI_StockStreamRequest);

            Assert.IsTrue(conv.CurrentState is InitialState_ConvI_StockStreamRequest);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);
            Assert.IsFalse(ConversationManager.ConversationExists(conv.Id));

            ConversationManager.AddConversation(conv);

            Assert.IsTrue(conv.CurrentState is InitialState_ConvI_StockStreamRequest);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);
            Assert.IsTrue(ConversationManager.ConversationExists(conv.Id));

            Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * 1.5));

            Assert.IsFalse(conv.CurrentState is InitialState_ConvI_StockStreamRequest);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Exactly(2));
            mock.Verify(state => state.HandleTimeout(), Times.Exactly(1));
            Assert.IsTrue(ConversationManager.ConversationExists(conv.Id));
        }

        [TestMethod]
        public void RequestTimeout()
        {
            int processId = 1;
            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

            var conv = new ConvI_StockStreamRequest(processId);

            //setup mock with response message
            var mock = new Mock<InitialState_ConvI_StockStreamRequest>(conv) { CallBase = true };
            mock.Setup(st => st.Send())
                .Callback(() =>
                {
                    //Pretend server isn't there, do nothing...
                });

            conv.SetInitialState(mock.Object as InitialState_ConvI_StockStreamRequest);

            Assert.IsTrue(conv.CurrentState is InitialState_ConvI_StockStreamRequest);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);
            Assert.IsFalse(ConversationManager.ConversationExists(conv.Id));

            ConversationManager.AddConversation(conv);

            Assert.IsTrue(conv.CurrentState is InitialState_ConvI_StockStreamRequest);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);
            Assert.IsTrue(ConversationManager.ConversationExists(conv.Id));

            Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * (Config.GetInt(Config.DEFAULT_RETRY_COUNT) + 1) * 1.2));

            Assert.IsFalse(conv.CurrentState is InitialState_ConvI_StockStreamRequest);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.HandleMessage(It.IsAny<Envelope>()), Times.Never);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Exactly(3));
            mock.Verify(state => state.HandleTimeout(), Times.Exactly(3));
            Assert.IsTrue(ConversationManager.ConversationExists(conv.Id));
        }
    }
}
