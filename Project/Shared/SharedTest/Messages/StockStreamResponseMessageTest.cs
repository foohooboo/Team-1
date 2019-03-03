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

            Assert.IsNull(stockStreamResponse.MarketDayList.Date);
            Assert.AreEqual(stockStreamResponse.MarketDayList.Data.Count, 0);
        }

        [TestMethod]
        public void InitializerTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 }; 
            string date = "1990-02-20";

            var MarketDay = new MarketDay
            {
                Date = date
            };
            MarketDay.Data.AddRange(stocks);

            var stockStreamResponse = new StockStreamResponseMessage
            {
                MarketDayList = MarketDay
            };

            Assert.AreEqual(stockStreamResponse.MarketDayList.Date, date);
            Assert.AreEqual(stockStreamResponse.MarketDayList.Data.Count, 2);
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
            string date = "1990-02-20";

            var MarketDay = new MarketDay
            {
                Date = date
            };
            MarketDay.Data.AddRange(stocks);

            var stockStreamResponse = new StockStreamResponseMessage
            {
                MarketDayList = MarketDay
            };

            var serializedMessage = stockStreamResponse.Encode();
            var deserializedMessage = MessageFactory.GetMessage(serializedMessage) as StockStreamResponseMessage;

            Assert.AreEqual(stockStreamResponse.MarketDayList.Data.Count, deserializedMessage.MarketDayList.Data.Count);
            Assert.AreEqual(stockStreamResponse.MarketDayList.Data[0].Close, deserializedMessage.MarketDayList.Data[0].Close);
            Assert.AreEqual(stockStreamResponse.MarketDayList.Date, deserializedMessage.MarketDayList.Date);
        }
    }
}
