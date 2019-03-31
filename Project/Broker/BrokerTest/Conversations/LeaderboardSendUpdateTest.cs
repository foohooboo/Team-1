using Broker;
using Broker.Conversations.CreatePortfolio;
using Broker.Conversations.GetPortfolio;
using Broker.Conversations.GetPortfolioResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using System.Linq;

namespace BrokerTest.Conversations
{
    [TestClass]
    public class LeaderboardSendUpdateTest
    {
        private Mock<GetPortfolioReceiveState> mock;

        [TestInitialize]
        public void TestInitialize()
        {
            PostOffice.AddBox("0.0.0.0:0");
            ConversationManager.Start(null);

            //create fake portfolios to populate leaderboard
            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);
            var asset = new Asset(testStock, 10);

            PortfolioManager.TryToCreate("port1", "pass1", out Portfolio port);
            port.ModifyAsset(asset);
            PortfolioManager.ReleaseLock(port);

            PortfolioManager.TryToCreate("port2", "pass2", out port);
            asset.Quantity--;
            port.ModifyAsset(asset);
            PortfolioManager.ReleaseLock(port);

            PortfolioManager.TryToCreate("port3", "pass3", out port);
            asset.Quantity--;
            port.ModifyAsset(asset);
            PortfolioManager.ReleaseLock(port);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ConversationManager.Stop();
            PostOffice.RemoveBox("0.0.0.0:0");

            foreach(Portfolio p in PortfolioManager.Portfolios.Values)
            {
                PortfolioManager.TryToRemove(p.PortfolioID);
            }
            Assert.IsTrue(PortfolioManager.Portfolios.Count == 0);
        }


        [TestMethod]
        public void Succeed()
        {
            


            //setup response message and mock
            //var mock = new Mock<InitTransactionStartingState>(conv) { CallBase = true };
            //mock.Setup(st => st.Send())//Pretend message is sent and response comes back...
            //    .Callback(() => {
            //        var responseMessage = new PortfolioUpdateMessage() { ConversationID = conv.Id, MessageID = "responceMessageID1234" };
            //        var responseEnv = new Envelope(responseMessage);
            //        ConversationManager.ProcessIncomingMessage(responseEnv);
            //    }).CallBase().Verifiable();

            ////execute test
            //conv.SetInitialState(mock.Object as InitTransactionStartingState);

            //Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
            //mock.Verify(state => state.Prepare(), Times.Never);
            //mock.Verify(state => state.Send(), Times.Never);

            //ConversationManager.AddConversation(conv);

            //Assert.IsFalse(conv.CurrentState is InitTransactionStartingState);
            //Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            //mock.Verify(state => state.Prepare(), Times.Once);
            //mock.Verify(state => state.Send(), Times.Once);
        }

        [TestMethod]
        public void Timeout()
        {
            //int portfolioId = 42;
            //var testStock = new Stock("TST", "Test Stock");
            //var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

            //var conv = new InitiateTransactionConversation(portfolioId, vStock, 1);

            ////setup response message and mock
            //var mock = new Mock<InitTransactionStartingState>(conv) { CallBase = true };
            //mock.Setup(prep => prep.Prepare()).Verifiable();//ensure DoPrepare is called.
            //mock.Setup(st => st.OnHandleMessage(It.IsAny<Envelope>())).CallBase();//Skip mock's HandleMessage override.
            //mock.Setup(st => st.Send()).CallBase().Verifiable();

            ////execute test
            //conv.SetInitialState(mock.Object as InitTransactionStartingState);

            //Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
            //mock.Verify(state => state.Prepare(), Times.Never);
            //mock.Verify(state => state.Send(), Times.Never);

            //ConversationManager.AddConversation(conv);

            //Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);
            //mock.Verify(state => state.Prepare(), Times.Once);
            //mock.Verify(state => state.Send(), Times.Once);
            //mock.Verify(state => state.HandleTimeout(), Times.Never);

            //Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * (Config.GetInt(Config.DEFAULT_RETRY_COUNT) + 1) * 1.1));

            //Assert.IsFalse(conv.CurrentState is InitTransactionStartingState);
            //Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            //mock.Verify(state => state.HandleMessage(It.IsAny<Envelope>()), Times.Never);
            //mock.Verify(state => state.Prepare(), Times.Once);
            //mock.Verify(state => state.Send(), Times.Exactly(3));
            //mock.Verify(state => state.HandleTimeout(), Times.Exactly(3));
        }


    }
}