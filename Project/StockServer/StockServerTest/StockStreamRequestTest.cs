using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using StockServer.Conversations.StockStreamRequest;
using System.Net;

namespace SharedTest.Conversations
{
    /// <summary>
    /// Summary description for MessageFactoryTest
    /// </summary>
    [TestClass]
    public class StockStreamRequestTest
    {
        public StockStreamRequestTest()
        {

        }

        [ClassInitialize()]
        public static void Test_ConvR_StockStreamRequestInitialize(TestContext testContext)
        {
            ConversationManager.Start(BuildConversationFromMessage);
        }

        public static Conversation BuildConversationFromMessage(Envelope e)
        {
            Conversation conv = null;

            if (e.Contents is StockStreamRequestMessage)
            {
                return new ConvR_StockStreamRequest(e);
            }

            return conv;
        }
        
        [TestMethod]
        public void InvalidMessageResponderTest()
        {
            //Simulate remote application-level ids
            string incomingConversationID = "5-23";
            int remoteProcessId = 2;
            int remotePortfolioId = 3;

            //Create a fake incoming message to simulate a StockStreamRequest
            var message = MessageFactory.GetMessage<AckMessage>(remoteProcessId, remotePortfolioId);
            message.ConversationID = incomingConversationID;
            var messageEnvelope = new Envelope(message);

            //Handle "incoming" message
            var replyConversation = PostOffice.HandleIncomingMessage(messageEnvelope);

            //Verify conversation was NOT built from message
            Assert.IsNull(replyConversation);
            Assert.IsFalse(ConversationManager.ConversationExists(incomingConversationID));
        }
    }
}
