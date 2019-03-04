using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.Conversations.StockStreamRequest.Initiator;

namespace BrokerTest
{
    [TestClass]
    public class InitiateStockStreamRequestTest
    {

        string destAddress = $"{Config.GetString(Config.STOCK_SERVER_IP)}:{Config.GetString(Config.STOCK_SERVER_PORT)}";

        [TestInitialize]
        public void TestInitialize()
        {
            PostOffice.AddBox(destAddress);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            PostOffice.RemoveBox(destAddress);
        }

        [TestMethod]
        public void SucessfulStockStreamRequestTest()
        {
            //Create a new StockStreamRequestConv_Initor conversation
            var initialState  = new InitialState_ConvI_StockStreamRequest(Config.GetInt(Config.BROKER_PROCESS_NUM));
            var stockStreamConv = new ConvI_StockStreamRequest(initialState);
            ConversationManager.AddConversation(stockStreamConv);
            string conversationId = stockStreamConv.ConversationId;

            //Verify conversation exists in Conversation Manager
            Assert.IsTrue(ConversationManager.ConversationExists(conversationId));

            //Create fake response message and process it
            var stockStreamResponse = new Envelope(new StockStreamResponseMessage());
            stockStreamResponse.Contents.ConversationID = stockStreamConv.ConversationId;
            ConversationManager.ProcessIncomingMessage(stockStreamResponse);

            //Conversation over, ensure it has been removed from Conversation Manager
            Assert.IsFalse(ConversationManager.ConversationExists(conversationId));
        }
    }
}

