using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.Messages;
using Shared.MarketStructures;

namespace SharedTest.Messages
{
    [TestClass]
    public class StockPriceUpdateTest
    {
        public StockPriceUpdateTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var stockPriceUpdate = new StockPriceUpdate();

            Assert.IsNull(stockPriceUpdate.StocksList.TradedCompanies);
            Assert.IsNull(stockPriceUpdate.StocksList.Date);
        }

        [TestMethod]
        public void LoadedConstructorTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 };
            var date = "1990-02-20";

            var marketDay = new MarketDay(date, stocks);
            var stockPriceUpdate = new StockPriceUpdate(marketDay);

            Assert.AreEqual(3, stockPriceUpdate.StocksList.TradedCompanies.Count);
            Assert.AreEqual(stockPriceUpdate.StocksList.Date, date);
        }

        [TestMethod]
        public void InitializerTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 };
            var date = "1990-02-20";

            var marketDay = new MarketDay(date, stocks);
            var stockPriceUpdate = new StockPriceUpdate
            {
                StocksList = marketDay
            };

            Assert.AreEqual(3, stockPriceUpdate.StocksList.TradedCompanies.Count);
            Assert.AreEqual(stockPriceUpdate.StocksList.Date, date);
        }

        [TestMethod]
        public void InheritsMessageTest()
        {
            var stockPriceUpdate = new StockPriceUpdate();

            Assert.AreEqual(stockPriceUpdate.SourceID, 0);
            Assert.IsNull(stockPriceUpdate.ConversationID);
            Assert.IsNull(stockPriceUpdate.MessageID);
        }

        [TestMethod]
        public void SerializerTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 };
            var date = "1990-02-20";

            var marketDay = new MarketDay(date, stocks);
            var stockPriceUpdate = new StockPriceUpdate(marketDay);

            var serializedMessage = stockPriceUpdate.Encode();
            var deserializedMessage = MessageFactory.GetMessage(serializedMessage, false) as StockPriceUpdate;

            Assert.AreEqual(stockPriceUpdate.StocksList.TradedCompanies.Count, deserializedMessage.StocksList.TradedCompanies.Count);
            Assert.AreEqual(stockPriceUpdate.StocksList.Date, deserializedMessage.StocksList.Date);
        }
    }
}
