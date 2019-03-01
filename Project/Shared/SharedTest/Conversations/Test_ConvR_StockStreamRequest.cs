using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.StockStreamRequest;

namespace SharedTest.Conversations
{
    /// <summary>
    /// Summary description for MessageFactoryTest
    /// </summary>
    [TestClass]
    public class Test_ConvR_StockStreamRequest
    {
        public Test_ConvR_StockStreamRequest()
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

            if(e.Contents is StockStreamRequestMessage)
            {
                return new ConvR_StockStreamRequest(e);
            }

            return conv;
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ValidStockStreamResponseTest()
        {
            //Simulate remote application-level ids
            string incomingConversationID = "5-18";
            int remoteProcessId = 2;
            int remotePortfolioId = 3;

            //Create a fake incoming message to simulate a StockStreamRequest
            var message = MessageFactory.GetMessage<StockStreamRequestMessage>(remoteProcessId, remotePortfolioId);
            message.ConversationID = incomingConversationID;
            var messageEnvelope = new Envelope(message);

            //Handle "incoming" message
            var replyConversation = PostOffice.HandleIncomingMessage(messageEnvelope);

            //Verify conversation was built from message
            Assert.IsNotNull(replyConversation);
            Assert.IsTrue(replyConversation.ConversationId.Equals(incomingConversationID));

            //Verify conversation does NOT exist in Conversation Manager because it ends after sending reply message.
            Assert.IsFalse(ConversationManager.ConversationExists(incomingConversationID));
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
