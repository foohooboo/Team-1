using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
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

            Assert.IsNull(stockStreamResponse.EvaluatedStocksList.Date);
            Assert.AreEqual(stockStreamResponse.EvaluatedStocksList.Count, 0);
        }

        [TestMethod]
        public void InitializerTest()
        {
            var stock1 = new EvaluatedStock();
            var stock2 = new EvaluatedStock();
            EvaluatedStock[] stocks = { stock1, stock2 }; 
            string date = "1990-02-20";

            var evaluatedStocks = new EvaluatedStocks
            {
                Date = date
            };
            evaluatedStocks.AddRange(stocks);

            var stockStreamResponse = new StockStreamResponseMessage
            {
                EvaluatedStocksList = evaluatedStocks
            };

            Assert.AreEqual(stockStreamResponse.EvaluatedStocksList.Date, date);
            Assert.AreEqual(stockStreamResponse.EvaluatedStocksList.Count, 2);
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
            var stock1 = new EvaluatedStock();
            var stock2 = new EvaluatedStock();
            EvaluatedStock[] stocks = { stock1, stock2 };
            string date = "1990-02-20";

            var evaluatedStocks = new EvaluatedStocks
            {
                Date = date
            };
            evaluatedStocks.AddRange(stocks);

            var stockStreamResponse = new StockStreamResponseMessage
            {
                EvaluatedStocksList = evaluatedStocks
            };

            var serializedMessage = MessageFactory.GetMessage(stockStreamResponse.Encode()) as StockStreamResponseMessage;

            Assert.AreEqual(stockStreamResponse.EvaluatedStocksList.Count, serializedMessage.EvaluatedStocksList.Count);
            Assert.AreEqual(stockStreamResponse.EvaluatedStocksList[0].close, serializedMessage.EvaluatedStocksList[0].close);
            //TODO: Date is being dropped in the serializer for some reason...
            Assert.AreEqual(stockStreamResponse.EvaluatedStocksList.Date, serializedMessage.EvaluatedStocksList.Date);
        }
    }
}
