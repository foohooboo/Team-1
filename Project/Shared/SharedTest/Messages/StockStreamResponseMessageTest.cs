using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.MarketStructures;
using Shared.Comms.Messages;

namespace SharedTest.Messages
{
    [TestClass]
    public class StockStreamResponseMessageTest
    {
        public StockStreamResponseMessageTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var stockStreamResponse = new StockStreamResponseMessage();

            Assert.AreEqual(stockStreamResponse.RecentHistory.Count, 0);
        }

        [TestMethod]
        public void InitializerTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 }; 
            string date1 = "1990-02-20";
            string date2 = "1990-03-20";

            MarketDay day1 = new MarketDay(date1,stocks);
            MarketDay day2 = new MarketDay(date2,stocks);//not important that has same valuated stocks

            var recentHistory = new MarketSegment
            {
                day1,
                day2
            };

            var stockStreamResponse = new StockStreamResponseMessage
            {
                RecentHistory = recentHistory
            };
            
            Assert.AreEqual(stockStreamResponse.RecentHistory[0].Date, date1);
            Assert.AreEqual(stockStreamResponse.RecentHistory[1].Date, date2);
            Assert.AreEqual(stockStreamResponse.RecentHistory.Count, 2);
            Assert.AreEqual(3, stockStreamResponse.RecentHistory[0].TradedCompanies.Count);
            Assert.AreEqual(3, stockStreamResponse.RecentHistory[1].TradedCompanies.Count);
        }

        [TestMethod]
        public void InheritsMessageTest()
        {
            var stockStreamResponse = new StockStreamResponseMessage();

            Assert.IsNull(stockStreamResponse.ConversationID);
            Assert.IsNull(stockStreamResponse.MessageID);
            Assert.AreEqual(stockStreamResponse.SourceID, 0);
        }

        [TestMethod]
        public void SerializerTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 };
            string date1 = "1990-02-20";

            MarketDay day1 = new MarketDay(date1, stocks);

            var recentHistory = new MarketSegment
            {
                day1
            };

            var stockStreamResponse = new StockStreamResponseMessage
            {
                RecentHistory = recentHistory
            };

            var serializedMessage = stockStreamResponse.Encode();
            var deserializedMessage = MessageFactory.GetMessage(serializedMessage, false) as StockStreamResponseMessage;

            Assert.AreEqual(stockStreamResponse.RecentHistory[0].TradedCompanies.Count, deserializedMessage.RecentHistory[0].TradedCompanies.Count);
            Assert.AreEqual(stockStreamResponse.RecentHistory[0].TradedCompanies[0].Close, deserializedMessage.RecentHistory[0].TradedCompanies[0].Close);
            Assert.AreEqual(stockStreamResponse.RecentHistory[0].Date, deserializedMessage.RecentHistory[0].Date);
        }
    }
}
