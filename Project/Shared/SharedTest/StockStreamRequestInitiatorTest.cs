using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.StockStreamRequest.Initiator;

namespace SharedTest
{
    /// <summary>
    /// Summary description for MessageFactoryTest
    /// </summary>
    [TestClass]
    public class StockStreamRequestInitiatorTest
    {
        public StockStreamRequestInitiatorTest()
        {
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
        public void SucessfulStockStreamRequestTest()
        {
            //Simulate application-level ids
            //TODO: Should these be moved into the TestContext?? -Dsphar 2/22/19
            int processId = 1;
            int portfolioId = 3;

            //Create a new StockStreamRequestConv_Initor conversation
            Conversation stockStreamConv_initiator = ConversationManager.InitiateAndStoreConversation<StockStreamRequestConv_Initor>(processId, portfolioId);
            string conversationId = stockStreamConv_initiator.ConversationId;

            //Verify conversation exists in Conversation Manager
            Assert.IsTrue(ConversationManager.ConversationExists(conversationId));

            //Create fake response message and process it
            var stockStreamResponse = new Envelope(new StockStreamResponseMessage());
            stockStreamResponse.Contents.ConversationID = stockStreamConv_initiator.ConversationId;
            ConversationManager.ProcessIncomingMessage(stockStreamResponse);

            //Conversation over, ensure it has been removed from Conversation Manager
            Assert.IsFalse(ConversationManager.ConversationExists(conversationId));
        }

        //TODO: Add test which handles an error response
        //TODO: Add test which handles no response (timeout)
    }
}
