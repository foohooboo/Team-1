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
            Assert.AreEqual(stockStreamResponse.MarketDayList.Count, 0);
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
            MarketDay.AddRange(stocks);

            var stockStreamResponse = new StockStreamResponseMessage
            {
                MarketDayList = MarketDay
            };

            Assert.AreEqual(stockStreamResponse.MarketDayList.Date, date);
            Assert.AreEqual(stockStreamResponse.MarketDayList.Count, 2);
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
            MarketDay.AddRange(stocks);

            var stockStreamResponse = new StockStreamResponseMessage
            {
                MarketDayList = MarketDay
            };

            var serializedMessage = MessageFactory.GetMessage(stockStreamResponse.Encode()) as StockStreamResponseMessage;

            Assert.AreEqual(stockStreamResponse.MarketDayList.Count, serializedMessage.MarketDayList.Count);
            Assert.AreEqual(stockStreamResponse.MarketDayList[0].Close, serializedMessage.MarketDayList[0].Close);
            //TODO: Date is being dropped in the serializer for some reason...
            Assert.AreEqual(stockStreamResponse.MarketDayList.Date, serializedMessage.MarketDayList.Date);
        }
    }
}
