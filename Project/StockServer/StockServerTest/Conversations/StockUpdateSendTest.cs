using System.Collections;
using System.Net;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Client;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;
using StockServer.Conversations.StockUpdate;

namespace StockServerTest.Conversations
{
    [TestClass]
    [DoNotParallelize]
    public class StockUpdateSendTest
    {

        private readonly ValuatedStock user1VStock = new ValuatedStock()
        {
            Symbol = "U1STK",
            Name = "User 1 stock",
            Open = 1.0F,
            High = 1.0F,
            Low = 1.0F,
            Close = 1.0F,
            Volume = 1
        };

        private readonly ValuatedStock user2VStock = new ValuatedStock()
        {
            Symbol = "U2STK",
            Name = "User 2 stock",
            Open = 2.0F,
            High = 2.0F,
            Low = 2.0F,
            Close = 2.0F,
            Volume = 2
        };

        private readonly ValuatedStock user3VStock = new ValuatedStock()
        {
            Symbol = "U3STK",
            Name = "User 3 stock",
            Open = 3.0F,
            High = 3.0F,
            Low = 3.0F,
            Close = 3.0F,
            Volume = 3
        };

        [TestInitialize]
        public void TestInitialize()
        {
            PostOffice.AddBox("0.0.0.0:0");
            ConversationManager.Start(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ConversationManager.Stop();
            PostOffice.RemoveBox("0.0.0.0:0");
        }


        [TestMethod]
        public void Succeed()
        {
            var conv = new StockUpdateSendConversation(42);
            var Stocks = new MarketDay("Today", new ValuatedStock[] { user1VStock, user2VStock, user3VStock });
            IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse("111.11.2.3"), 124);

            //setup response message and mock
            var mock = new Mock<StockUpdateSendState>(Stocks, clientEndpoint, conv, null) { CallBase = true };
            mock.Setup(st => st.Send())//Pretend message is sent and response comes back...
                .Callback(() =>
                {
                    Stocks = ((mock.Object as StockUpdateSendState).OutboundMessage.Contents as StockPriceUpdate).StocksList;
                    var responseMessage = new AckMessage() { ConversationID = conv.Id, MessageID = "responseMessageID1234" };
                    var responseEnv = new Envelope(responseMessage);
                    ConversationManager.ProcessIncomingMessage(responseEnv);
                }).CallBase().Verifiable();

            ////execute test
            conv.SetInitialState(mock.Object as StockUpdateSendState);
            Assert.IsTrue(ClientManager.TryToAdd(clientEndpoint));

            Assert.IsTrue(conv.CurrentState is StockUpdateSendState);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);

            ConversationManager.AddConversation(conv);

            Assert.IsFalse(conv.CurrentState is StockUpdateSendState);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);


            // These fail becaue the traded compaines are not the passed in values from above.
            Assert.AreEqual("U1STK", Stocks.TradedCompanies[0].Symbol);
            Assert.AreEqual("U2STK", Stocks.TradedCompanies[1].Symbol);
            Assert.AreEqual("U3STK", Stocks.TradedCompanies[2].Symbol);

            var conv2 = new StockUpdateSendConversation(42);
            var Stocks2 = new MarketDay("Today", new ValuatedStock[] { user1VStock, user2VStock, user3VStock });

            //setup response message and mock
            var mock2 = new Mock<StockUpdateSendState>(Stocks2, clientEndpoint, conv2, null) { CallBase = true };
            mock2.Setup(st => st.Send())//Pretend message is sent and response comes back...
                .Callback(() =>
                {
                    Stocks2 = ((mock2.Object as StockUpdateSendState).OutboundMessage.Contents as StockPriceUpdate).StocksList;
                    var responseMessage = new AckMessage() { ConversationID = conv2.Id, MessageID = "responseMessageID1234" };
                    var responseEnv = new Envelope(responseMessage);
                    ConversationManager.ProcessIncomingMessage(responseEnv);
                }).CallBase().Verifiable();

            ////execute test
            conv2.SetInitialState(mock2.Object as StockUpdateSendState);

            Assert.IsTrue(conv2.CurrentState is StockUpdateSendState);
            mock2.Verify(state => state.Prepare(), Times.Never);
            mock2.Verify(state => state.Send(), Times.Never);

            ConversationManager.AddConversation(conv2);

            Assert.IsFalse(conv2.CurrentState is StockUpdateSendState);
            Assert.IsTrue(conv2.CurrentState is ConversationDoneState);
            mock2.Verify(state => state.Prepare(), Times.Once);
            mock2.Verify(state => state.Send(), Times.Once);

            // These fail becaue the traded compaines are not the passed in values from above.
            Assert.AreEqual("U1STK", Stocks2.TradedCompanies[0].Symbol);
            Assert.AreEqual("U2STK", Stocks2.TradedCompanies[1].Symbol);
            Assert.AreEqual("U3STK", Stocks2.TradedCompanies[2].Symbol);

            Assert.IsTrue(ClientManager.TryToRemove(clientEndpoint));
        }

        [TestMethod]
        public void Timeout()
        {
            var conv = new StockUpdateSendConversation(42);
            SortedList Records = new SortedList();
            var clientEndpoint = new IPEndPoint(IPAddress.Parse("111.11.2.3"), 124);

            ValuatedStock[] day1 = { user1VStock, user2VStock, user3VStock };
            var mday1 = new MarketDay("day-1", day1);
            //LeaderboardManager.Market = new MarketDay("day-1", day1);

            //setup response message and mock
            var mock = new Mock<StockUpdateSendState>(mday1, clientEndpoint, conv, null) { CallBase = true };
            mock.Setup(st => st.Send())//Pretend message is sent and response comes back...
                .Callback(() =>
                {
                    //pretend client never responds, do nothing
                }).CallBase().Verifiable();

            ////execute test
            conv.SetInitialState(mock.Object as StockUpdateSendState);
            var clients = ClientManager.Clients;
            Assert.IsTrue(ClientManager.TryToAdd(clientEndpoint));

            Assert.IsTrue(conv.CurrentState is StockUpdateSendState);
            mock.Verify(state => state.Prepare(), Times.Never);
            mock.Verify(state => state.Send(), Times.Never);

            ConversationManager.AddConversation(conv);

            Assert.IsTrue(conv.CurrentState is StockUpdateSendState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);

            Assert.AreEqual(1, ClientManager.Clients.Count);

            Thread.Sleep((int)(Config.GetInt(Config.DEFAULT_TIMEOUT) * (Config.GetInt(Config.DEFAULT_RETRY_COUNT) + 1) * 1.1));

            Assert.AreEqual(0, ClientManager.Clients.Count);

            Assert.IsFalse(conv.CurrentState is StockUpdateSendState);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.HandleMessage(It.IsAny<Envelope>()), Times.Never);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Exactly(3));
            mock.Verify(state => state.HandleTimeout(), Times.Exactly(3));
        }
    }
}