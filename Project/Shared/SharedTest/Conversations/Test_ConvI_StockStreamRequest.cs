using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.Conversations.StockStreamRequest.Initiator;

namespace SharedTest.Conversations
{


    [TestClass]
    public class Test_ConvI_StockStreamRequest
    {
        string address = $"{Config.GetString(Config.STOCK_SERVER_IP)}:{Config.GetInt(Config.STOCK_SERVER_PORT)}";

        [TestInitialize]
        public void TestInitialize(){
            PostOffice.AddBox(address);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            PostOffice.RemoveBox(address);
        }

        [TestMethod]
        public void SucessfulStockStreamRequestTest()
        {
            //Simulate application-level ids
            //TODO: Should these be moved into the TestContext?? -Dsphar 2/22/19
            int processId = 1;

            //Create a new StockStreamRequestConv_Initor conversation
            var stockStreamConv = new ConvI_StockStreamRequest(new InitialState_ConvI_StockStreamRequest(processId));
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

        //TODO: Add test which handles an error response (once error message is created)
        //TODO: Add test which handles no response (once timeout functionality added)
    }
}
