using System;
using System.Threading;
using Client.Conversations;
using Client.Conversations.CreatePortfolio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;

namespace ClientTest.Conversations
{
    [TestClass]
    public class CreatePortfolioTest
    {

        [TestInitialize]
        public void TestInitialize()
        {
            ComService.AddClient(Config.DEFAULT_UDP_CLIENT,0);
            ConversationManager.Start(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ConversationManager.Stop();
            ComService.RemoveClient(Config.DEFAULT_UDP_CLIENT);
        }

        [TestMethod]
        public void RequestSucceed()
        {
            int processIdId = 42;
            var username = "myUsername";
            var password = "myPassword";
                                             
            var conv = new CreatePortfolioRequestConversation(processIdId);

            //setup response message and mock
            var mock = new Mock<CreatePortfolioRequestState>(username, password, password, conv, null) { CallBase = true };
            mock.Setup(st => st.Send())//Pretend message is sent and response comes back...
                .Callback(()=> {
                var responseMessage = new PortfolioUpdateMessage() { ConversationID = conv.Id, MessageID = "responseMessageID1234" };
                var responseEnv = new Envelope(responseMessage);
                ConversationManager.ProcessIncomingMessage(responseEnv); 
            }).CallBase().Verifiable();
            
            //execute test
            conv.SetInitialState(mock.Object as CreatePortfolioRequestState);

            Assert.IsTrue(conv.CurrentState is CreatePortfolioRequestState);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);

            ConversationManager.AddConversation(conv);

            Assert.IsFalse(conv.CurrentState is CreatePortfolioRequestState);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
        }

        [TestMethod]
        public void RequestSingleTimeoutThenSucceed()
        {
            int portfolioId = 42;
            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

            var conv = new InitiateTransactionConversation(portfolioId, vStock, 1);
            int requests = 0;

            //setup response message and mock
            var mock = new Mock<InitTransactionStartingState>(conv) { CallBase = true };
            mock.Setup(prep => prep.Prepare()).Verifiable();//ensure DoPrepare is called.
            mock.Setup(st => st.OnHandleMessage(It.IsAny<Envelope>(),0)).CallBase();//Skip mock's HandleMessage override.
            mock.Setup(st => st.Send())//Pretend message is sent and response comes back...
                .Callback(() => {
                    if (++requests > 1)
                    {
                        var responseMessage = new PortfolioUpdateMessage() { ConversationID = conv.Id };
                        var responseEnv = new Envelope(responseMessage);
                        ConversationManager.ProcessIncomingMessage(responseEnv);
                    }
                }).CallBase().Verifiable();

            //execute test
            conv.SetInitialState(mock.Object as InitTransactionStartingState);

            Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);

            ConversationManager.AddConversation(conv);

            Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);

            Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * 1.5));

            Assert.IsFalse(conv.CurrentState is InitTransactionStartingState);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.AtLeast(2));
            mock.Verify(state => state.HandleTimeout(), Times.AtLeast(1));
        }

        [TestMethod]
        public void RequestTimeout()
        {
            int portfolioId = 42;
            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

            var conv = new InitiateTransactionConversation(portfolioId, vStock, 1);

            //setup response message and mock
            var mock = new Mock<InitTransactionStartingState>(conv) { CallBase = true };
            mock.Setup(prep => prep.Prepare()).Verifiable();//ensure DoPrepare is called.
            mock.Setup(st => st.OnHandleMessage(It.IsAny<Envelope>(),0)).CallBase();//Skip mock's HandleMessage override.
            mock.Setup(st => st.Send()).CallBase().Verifiable();

            //execute test
            conv.SetInitialState(mock.Object as InitTransactionStartingState);

            Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);

            ConversationManager.AddConversation(conv);

            Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);

            Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * (Config.GetInt(Config.DEFAULT_RETRY_COUNT)+1)*1.1 ));

            Assert.IsFalse(conv.CurrentState is InitTransactionStartingState);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.HandleMessage(It.IsAny<Envelope>()), Times.Never);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.AtLeast(3));
            mock.Verify(state => state.HandleTimeout(), Times.AtLeast(3));
        }
    }
}
