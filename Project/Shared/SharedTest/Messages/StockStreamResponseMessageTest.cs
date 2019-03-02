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

            Assert.IsNull(stockStreamResponse.StockMarketDayList.Date);
            Assert.AreEqual(stockStreamResponse.StockMarketDayList.Count, 0);
        }

        [TestMethod]
        public void InitializerTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 }; 
            string date = "1990-02-20";

            var StockMarketDay = new StockMarketDay
            {
                Date = date
            };
            StockMarketDay.AddRange(stocks);

            var stockStreamResponse = new StockStreamResponseMessage
            {
                StockMarketDayList = StockMarketDay
            };

            Assert.AreEqual(stockStreamResponse.StockMarketDayList.Date, date);
            Assert.AreEqual(stockStreamResponse.StockMarketDayList.Count, 2);
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

            var StockMarketDay = new StockMarketDay
            {
                Date = date
            };
            StockMarketDay.AddRange(stocks);

            var stockStreamResponse = new StockStreamResponseMessage
            {
                StockMarketDayList = StockMarketDay
            };

            var serializedMessage = MessageFactory.GetMessage(stockStreamResponse.Encode()) as StockStreamResponseMessage;

            Assert.AreEqual(stockStreamResponse.StockMarketDayList.Count, serializedMessage.StockMarketDayList.Count);
            Assert.AreEqual(stockStreamResponse.StockMarketDayList[0].Close, serializedMessage.StockMarketDayList[0].Close);
            //TODO: Date is being dropped in the serializer for some reason...
            Assert.AreEqual(stockStreamResponse.StockMarketDayList.Date, serializedMessage.StockMarketDayList.Date);
        }
    }
}
