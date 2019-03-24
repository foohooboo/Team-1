using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.Conversations.StockStreamRequest.Initiator;
using System.Threading;

namespace BrokerTest
{
    [TestClass]
    public class InitiateStockStreamRequestTest
    {

        readonly string destAddress = $"{Config.GetString(Config.STOCK_SERVER_IP)}:{Config.GetString(Config.STOCK_SERVER_PORT)}";

        [TestInitialize]
        public void TestInitialize()
        {
            PostOffice.AddBox("0.0.0.0:0");
            ConversationManager.Start(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            PostOffice.RemoveBox("0.0.0.0:0");
            ConversationManager.Stop();
        }

        [TestMethod]
        public void SucessfulStockStreamRequestTest()
        {
            //Create a new StockStreamRequestConv_Initor conversation
            var stockStreamConv = new ConvI_StockStreamRequest(42);
            var initialState = new InitialState_ConvI_StockStreamRequest(stockStreamConv);
            stockStreamConv.SetInitialState(initialState);
            ConversationManager.AddConversation(stockStreamConv);
            string conversationId = stockStreamConv.Id;

            //Verify conversation exists in Conversation Manager
            Assert.IsTrue(ConversationManager.ConversationExists(conversationId));

            //Create fake response message and process it
            var stockStreamResponse = new Envelope(new StockStreamResponseMessage());
            stockStreamResponse.Contents.ConversationID = stockStreamConv.Id;
            ConversationManager.ProcessIncomingMessage(stockStreamResponse);

            //Conversation over but we hold onto the done state for a little...
            //ensure it has not yet been removed from Conversation Manager
            Assert.IsTrue(ConversationManager.ConversationExists(conversationId));

            var retrycount = Config.GetInt(Config.DEFAULT_RETRY_COUNT);
            var timeout = Config.GetInt(Config.DEFAULT_TIMEOUT);
            Thread.Sleep(retrycount * timeout * 2);

            //Conversation should have cleaned itself up now...
            Assert.IsFalse(ConversationManager.ConversationExists(conversationId));
        }
    }
}

