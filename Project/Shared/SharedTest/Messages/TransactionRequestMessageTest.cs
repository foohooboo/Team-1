using Shared.Comms.Messages;
using Shared.MarketStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.Messages
{
    [TestClass]
    public class TransactionRequestMessageTest
    {
        public TransactionRequestMessageTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var transactionRequest = new TransactionRequestMessage();

            Assert.AreEqual(transactionRequest.Quantity, 0);
            Assert.AreEqual(transactionRequest.StockValue.Close, 0.0F);
            Assert.AreEqual(transactionRequest.StockValue.Open, 0.0F);
            Assert.AreEqual(transactionRequest.StockValue.High, 0.0F);
            Assert.AreEqual(transactionRequest.StockValue.Low, 0.0F);
            Assert.AreEqual(transactionRequest.StockValue.Volume, 0);
            Assert.IsNull(transactionRequest.StockValue.Symbol);
            Assert.IsNull(transactionRequest.StockValue.Name);
        }

        [TestMethod]
        public void CopyConstructorTest()
        {
            ValuatedStock stock = new ValuatedStock();

            stock.Close = 1.0F;
            stock.Open = 1.1F;
            stock.High = 1.2F;
            stock.Low = 1.3F;
            stock.Volume = 2;
            stock.Symbol = "GOOGL";
            stock.Name = "GOOGLE";

            var request1 = new TransactionRequestMessage(2, stock);
            var request2 = new TransactionRequestMessage(request1);

            Assert.AreEqual(request1.Quantity, request1.Quantity);
            Assert.AreEqual(request1.StockValue.Close, request1.StockValue.Close);
            Assert.AreEqual(request1.StockValue.Open, request1.StockValue.Open);
            Assert.AreEqual(request1.StockValue.High, request1.StockValue.High);
            Assert.AreEqual(request1.StockValue.Low, request1.StockValue.Low);
            Assert.AreEqual(request1.StockValue.Volume, request1.StockValue.Volume);
            Assert.AreEqual(request1.StockValue.Symbol, request1.StockValue.Symbol);
            Assert.AreEqual(request1.StockValue.Name, request1.StockValue.Name);
        }

        [TestMethod]
        public void LoadedConstructorTest()
        {
            ValuatedStock stock = new ValuatedStock();
            
            stock.Close = 1.0F;
            stock.Open = 1.1F;
            stock.High = 1.2F;
            stock.Low = 1.3F;
            stock.Volume = 2;
            stock.Symbol = "GOOGL";
            stock.Name = "GOOGLE";

            var transactionRequest = new TransactionRequestMessage(2, stock);

            Assert.AreEqual(transactionRequest.Quantity, 2);
            Assert.AreEqual(transactionRequest.StockValue.Close, 1.0F);
            Assert.AreEqual(transactionRequest.StockValue.Open, 1.1F);
            Assert.AreEqual(transactionRequest.StockValue.High, 1.2F);
            Assert.AreEqual(transactionRequest.StockValue.Low, 1.3F);
            Assert.AreEqual(transactionRequest.StockValue.Volume, 2);
            Assert.AreEqual(transactionRequest.StockValue.Symbol, "GOOGL");
            Assert.AreEqual(transactionRequest.StockValue.Name, "GOOGLE");
        }

        [TestMethod]
        public void InitializerTest()
        {
            var stock = new ValuatedStock
            {
                Close = 1.0F,
                Open = 1.1F,
                High = 1.2F,
                Low = 1.3F,
                Volume = 2,
                Symbol = "GOOGL",
                Name = "GOOGLE"
            };

            var transactionRequest = new TransactionRequestMessage
            {
                Quantity = 2,
                StockValue = stock
            };

            Assert.AreEqual(transactionRequest.Quantity, 2);
            Assert.AreEqual(transactionRequest.StockValue.Close, 1.0F);
            Assert.AreEqual(transactionRequest.StockValue.Open, 1.1F);
            Assert.AreEqual(transactionRequest.StockValue.High, 1.2F);
            Assert.AreEqual(transactionRequest.StockValue.Low, 1.3F);
            Assert.AreEqual(transactionRequest.StockValue.Volume, 2);
            Assert.AreEqual(transactionRequest.StockValue.Symbol, "GOOGL");
            Assert.AreEqual(transactionRequest.StockValue.Name, "GOOGLE");
        }

        [TestMethod]
        public void InheritsMessageTest()
        {
            var transactionRequest = new TransactionRequestMessage();

            Assert.AreEqual(transactionRequest.SourceID, 0);
            Assert.IsNull(transactionRequest.MessageID);
            Assert.IsNull(transactionRequest.ConversationID);
        }

        [TestMethod]
        public void SerializerTest()
        {
            ValuatedStock stock = new ValuatedStock();

            stock.Close = 1.0F;
            stock.Open = 1.1F;
            stock.High = 1.2F;
            stock.Low = 1.3F;
            stock.Volume = 2;
            stock.Symbol = "GOOGL";
            stock.Name = "GOOGLE";

            var request1 = new TransactionRequestMessage(2, stock);

            var serializedMessage = request1.Encode();
            var deserializedMessage = MessageFactory.GetMessage(serializedMessage) as TransactionRequestMessage;

            Assert.AreEqual(request1.Quantity, deserializedMessage.Quantity);
            Assert.AreEqual(request1.StockValue.Close, deserializedMessage.StockValue.Close);
            Assert.AreEqual(request1.StockValue.Open, deserializedMessage.StockValue.Open);
            Assert.AreEqual(request1.StockValue.High, deserializedMessage.StockValue.High);
            Assert.AreEqual(request1.StockValue.Low, deserializedMessage.StockValue.Low);
            Assert.AreEqual(request1.StockValue.Volume, deserializedMessage.StockValue.Volume);
            Assert.AreEqual(request1.StockValue.Symbol, deserializedMessage.StockValue.Symbol);
            Assert.AreEqual(request1.StockValue.Name, deserializedMessage.StockValue.Name);
        }
    }
}
