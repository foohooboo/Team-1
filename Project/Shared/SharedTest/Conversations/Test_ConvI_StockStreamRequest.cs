using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.Conversations.StockStreamRequest.Initiator;
using System.Threading;

namespace SharedTest.Conversations
{


    [TestClass]
    public class Test_ConvI_StockStreamRequest
    {
        [TestInitialize]
        public void TestInitialize(){
            ConversationManager.Start(null);
            PostOffice.AddBox("0.0.0.0:0");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            PostOffice.RemoveBox("0.0.0.0:0");
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
            var stockStreamResponse = new Envelope(new StockStreamResponseMessage());
            stockStreamResponse.Contents.ConversationID = stockStreamConv.Id;
            ConversationManager.ProcessIncomingMessage(stockStreamResponse);

            //Conversation over but we hold onto the done state for a little
            //in case we need to handle a retry. Ensure it has not yet been
            //removed from Conversation Manager
            Assert.IsTrue(ConversationManager.ConversationExists(conversationId));

            var retrycount = Config.GetInt(Config.DEFAULT_RETRY_COUNT);
            var timeout = Config.GetInt(Config.DEFAULT_TIMEOUT);
            Thread.Sleep(retrycount*timeout*2);

            //Conversation should have cleaned itself up now...
            Assert.IsFalse(ConversationManager.ConversationExists(conversationId));
        }

        //TODO: Add test which handles an error response (once error message is created)
        //TODO: Add test which handles no response (once timeout functionality added)
    }
}
