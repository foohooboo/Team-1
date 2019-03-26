using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.MarketStructures;
using StockServer.Conversations.StockStreamRequest;

namespace SharedTest.Conversations
{
    /// <summary>
    /// Summary description for MessageFactoryTest
    /// </summary>
    [TestClass]
    public class StockStreamRequestTest
    {
        //private Mock<RespondStockStreamRequest_InitialState> mock;

        //public Conversation ConversationBuilder(Envelope env)
        //{
        //    Conversation conv = null;

        //    switch (env.Contents)
        //    {
        //        case TransactionRequestMessage m:
        //            conv = new ConvR_StockStreamRequest(env);

        //            //setup response message as mock
        //            mock = new Mock<RespondStockStreamRequest_InitialState>(conv, m.MessageID)  { CallBase = true };
        //            mock.Setup(prep => prep.DoPrepare()).CallBase().Verifiable();
        //            mock.Setup(st => st.OnHandleMessage(It.IsAny<Envelope>())).CallBase().Verifiable();
        //            mock.Setup(st => st.Send()).CallBase().Verifiable();

        //            conv.SetInitialState(mock.Object as RespondTransaction_InitialState);

        //            break;
        //    }

        //    return conv;
        //}



        //[ClassInitialize()]
        //public static void Test_ConvR_StockStreamRequestInitialize(TestContext testContext)
        //{
        //    ConversationManager.Start(BuildConversationFromMessage);
        //}

        //public static Conversation BuildConversationFromMessage(Envelope e)
        //{
        //    Conversation conv = null;

        //    if (e.Contents is StockStreamRequestMessage)
        //    {
        //        return new ConvR_StockStreamRequest(e);
        //    }

        //    return conv;
        //}

        //[TestMethod]
        //public void InvalidMessageResponderTest()
        //{
        //    //Simulate remote application-level ids
        //    string incomingConversationID = "5-23";
        //    int remoteProcessId = 2;
        //    int remotePortfolioId = 3;

        //    //Create a fake incoming message to simulate an ack StockStreamRequest
        //    var message = MessageFactory.GetMessage<AckMessage>(remoteProcessId, remotePortfolioId);
        //    message.ConversationID = incomingConversationID;
        //    var messageEnvelope = new Envelope(message);

        //    //Handle "incoming" message
        //    var replyConversation = PostOffice.HandleIncomingMessage(messageEnvelope);

        //    //Verify conversation was NOT built from message
        //    Assert.IsNull(replyConversation);
        //    Assert.IsFalse(ConversationManager.ConversationExists(incomingConversationID));
        //}

        //[TestMethod]
        //public void RequestSucceed()
        //{
        //    int portfolioId = 42;
        //    var testStock = new Stock("TST", "Test Stock");
        //    var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

        //    var conv = new InitiateTransactionConversation(portfolioId, vStock, 1);

        //    //setup response message and mock
        //    var mock = new Mock<InitTransactionStartingState>(conv)  { CallBase = true };
        //    mock.Setup(prep => prep.DoPrepare()).Verifiable();//ensure DoPrepare is called.
        //    mock.Setup(st => st.HandleMessage(It.IsAny<Envelope>())).CallBase();//Skip mock's HandleMessage override.
        //    mock.Setup(st => st.Send())//Pretend message is sent and response comes back...
        //        .Callback(() =>
        //        {
        //            var responseMessage = new PortfolioUpdateMessage() { ConversationID = conv.Id };
        //            var responseEnv = new Envelope(responseMessage);
        //            ConversationManager.ProcessIncomingMessage(responseEnv);
        //        });

        //    //execute test
        //    conv.SetInitialState(mock.Object as InitTransactionStartingState);

        //    Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
        //    mock.Verify(state => state.DoPrepare(), Times.Never);
        //    mock.Verify(state => state.Send(), Times.Never);

        //    ConversationManager.AddConversation(conv);

        //    Assert.IsFalse(conv.CurrentState is InitTransactionStartingState);
        //    Assert.IsTrue(conv.CurrentState is ConversationDoneState);
        //    mock.Verify(state => state.DoPrepare(), Times.Once);
        //    mock.Verify(state => state.Send(), Times.Once);
        //}

        //[TestMethod]
        //public void RequestSingleTimeoutThenSucceed()
        //{
        //    int portfolioId = 42;
        //    var testStock = new Stock("TST", "Test Stock");
        //    var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

        //    var conv = new InitiateTransactionConversation(portfolioId, vStock, 1);
        //    int requests = 0;

        //    //setup response message and mock
        //    var mock = new Mock<InitTransactionStartingState>(conv) { CallBase = true };
        //    mock.Setup(prep => prep.DoPrepare()).Verifiable();//ensure DoPrepare is called.
        //    mock.Setup(st => st.HandleMessage(It.IsAny<Envelope>())).CallBase();//Skip mock's HandleMessage override.
        //    mock.Setup(st => st.Send())//Pretend message is sent and response comes back...
        //        .Callback(() =>
        //        {
        //            if (++requests > 1)
        //            {
        //                var responseMessage = new PortfolioUpdateMessage() { ConversationID = conv.Id };
        //                var responseEnv = new Envelope(responseMessage);
        //                ConversationManager.ProcessIncomingMessage(responseEnv);
        //            }
        //        });

        //    //execute test
        //    conv.SetInitialState(mock.Object as InitTransactionStartingState);

        //    Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
        //    mock.Verify(state => state.DoPrepare(), Times.Never);
        //    mock.Verify(state => state.Send(), Times.Never);

        //    ConversationManager.AddConversation(conv);

        //    Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
        //    mock.Verify(state => state.DoPrepare(), Times.Once);
        //    mock.Verify(state => state.Send(), Times.Once);
        //    mock.Verify(state => state.HandleTimeout(), Times.Never);

        //    Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * 1.5));

        //    Assert.IsFalse(conv.CurrentState is InitTransactionStartingState);
        //    Assert.IsTrue(conv.CurrentState is ConversationDoneState);
        //    mock.Verify(state => state.DoPrepare(), Times.Once);
        //    mock.Verify(state => state.Send(), Times.Exactly(2));
        //    mock.Verify(state => state.HandleTimeout(), Times.Exactly(1));
        //}

        //[TestMethod]
        //public void RequestTimeout()
        //{
        //    int portfolioId = 42;
        //    var testStock = new Stock("TST", "Test Stock");
        //    var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

        //    var conv = new InitiateTransactionConversation(portfolioId, vStock, 1);

        //    //setup response message and mock
        //    var mock = new Mock<InitTransactionStartingState>(conv) { CallBase = true };
        //    mock.Setup(prep => prep.DoPrepare()).Verifiable();//ensure DoPrepare is called.
        //    mock.Setup(st => st.HandleMessage(It.IsAny<Envelope>())).CallBase();//Skip mock's HandleMessage override.
        //    mock.Setup(st => st.Send()).Verifiable();

        //    //execute test
        //    conv.SetInitialState(mock.Object as InitTransactionStartingState);

        //    Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
        //    mock.Verify(state => state.DoPrepare(), Times.Never);
        //    mock.Verify(state => state.Send(), Times.Never);

        //    ConversationManager.AddConversation(conv);

        //    Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
        //    mock.Verify(state => state.DoPrepare(), Times.Once);
        //    mock.Verify(state => state.Send(), Times.Once);
        //    mock.Verify(state => state.HandleTimeout(), Times.Never);

        //    Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * (Config.GetInt(Config.DEFAULT_RETRY_COUNT) + 1) * 1.5));

        //    Assert.IsFalse(conv.CurrentState is InitTransactionStartingState);
        //    Assert.IsTrue(conv.CurrentState is ConversationDoneState);
        //    mock.Verify(state => state.HandleMessage(It.IsAny<Envelope>()), Times.Never);
        //    mock.Verify(state => state.DoPrepare(), Times.Once);
        //    mock.Verify(state => state.Send(), Times.Exactly(3));
        //    mock.Verify(state => state.HandleTimeout(), Times.Exactly(3));
        //}


    }
}
