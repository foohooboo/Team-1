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

            Assert.AreEqual(stockPriceUpdate.StocksList.Data.Count, 0);
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

            Assert.AreEqual(stockPriceUpdate.StocksList.Data.Count, 2);
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

            Assert.AreEqual(stockPriceUpdate.StocksList.Data.Count, 2);
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
            var deserializedMessage = MessageFactory.GetMessage(serializedMessage) as StockPriceUpdate;

            Assert.AreEqual(stockPriceUpdate.StocksList.Data.Count, deserializedMessage.StocksList.Data.Count);
            Assert.AreEqual(stockPriceUpdate.StocksList.Date, deserializedMessage.StocksList.Date);
        }
    }
}
