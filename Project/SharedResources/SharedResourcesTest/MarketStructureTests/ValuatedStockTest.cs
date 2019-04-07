using Shared.MarketStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.MarketStructureTests
{
    [TestClass]
    public class ValuatedStockTest
    {
        public ValuatedStockTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var ValuatedStock = new ValuatedStock();

            Assert.AreEqual(ValuatedStock.Close, 0.0F);
            Assert.AreEqual(ValuatedStock.High, 0.0F);
            Assert.AreEqual(ValuatedStock.Low, 0.0F);
            Assert.AreEqual(ValuatedStock.Open, 0.0F);
            Assert.AreEqual(ValuatedStock.Volume, 0);
            Assert.IsNull(ValuatedStock.Name);
            Assert.IsNull(ValuatedStock.Symbol);
        }

        [TestMethod]
        public void LoadedConstructorTest()
        {
            var stock = new Stock("GOOGL","GOOGLE");
            string[] data = { "yolo", "1.0", "1.1", "1.2", "1.3", "2" }; 
            var ValuatedStock = new ValuatedStock(data, stock);

            Assert.AreEqual(ValuatedStock.Open, 1.0F);
            Assert.AreEqual(ValuatedStock.High, 1.1F);
            Assert.AreEqual(ValuatedStock.Low, 1.2F);
            Assert.AreEqual(ValuatedStock.Close, 1.3F);
            Assert.AreEqual(ValuatedStock.Volume, 2);
            Assert.AreEqual(ValuatedStock.Name, "GOOGLE");
            Assert.AreEqual(ValuatedStock.Symbol, "GOOGL");
        }

        [TestMethod]
        public void InitializerTest()
        {
            var ValuatedStock = new ValuatedStock
            {
                Symbol = "GOOGL",
                Name = "GOOGLE",
                Open = 1.0F,
                High = 1.1F,
                Low = 1.2F,
                Close = 1.3F,
                Volume = 2
            };

            Assert.AreEqual(ValuatedStock.Open, 1.0F);
            Assert.AreEqual(ValuatedStock.High, 1.1F);
            Assert.AreEqual(ValuatedStock.Low, 1.2F);
            Assert.AreEqual(ValuatedStock.Close, 1.3F);
            Assert.AreEqual(ValuatedStock.Volume, 2);
            Assert.AreEqual(ValuatedStock.Name, "GOOGLE");
            Assert.AreEqual(ValuatedStock.Symbol, "GOOGL");
        }

        [TestMethod]
        public void EqualOverrideTest()
        {

            var stock1 = new ValuatedStock()
            {
                Symbol = "STK",
                Name = "Stock",
                Open = 6,
                High = 7,
                Low = 8,
                Close = 9,
                Volume = 10
            };

            var stock2 = new ValuatedStock()
            {
                Symbol = "STK",
                Name = "Stock",
                Open = 6,
                High = 7,
                Low = 8,
                Close = 9,
                Volume = 10
            };

            //Equals
            Assert.IsTrue(stock1.Equals(stock2));


            //Bad stock symbol
            stock2 = new ValuatedStock()
            {
                Symbol = "STK2BADD",
                Name = "Stock 2",
                Open = 6,
                High = 7,
                Low = 8,
                Close = 9,
                Volume = 10
            };
            Assert.IsFalse(stock1.Equals(stock2));

            //Bad stock name
            stock2 = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2 BADD",
                Open = 6,
                High = 7,
                Low = 8,
                Close = 9,
                Volume = 10
            };
            Assert.IsFalse(stock1.Equals(stock2));

            //Bad stock open
            stock2 = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2",
                Open = 6000,
                High = 7,
                Low = 8,
                Close = 9,
                Volume = 10
            };
            Assert.IsFalse(stock1.Equals(stock2));

            //Bad stock High
            stock2 = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2",
                Open = 6,
                High = 7000,
                Low = 8,
                Close = 9,
                Volume = 10
            };
            Assert.IsFalse(stock1.Equals(stock2));

            //Bad stock Low
            stock2 = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2",
                Open = 6,
                High = 7,
                Low = 8000,
                Close = 9,
                Volume = 10
            };
            Assert.IsFalse(stock1.Equals(stock2));

            //Bad stock Close
            stock2 = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2",
                Open = 6,
                High = 7,
                Low = 8,
                Close = 9000,
                Volume = 10
            };
            Assert.IsFalse(stock1.Equals(stock2));

            //Bad stock Volume
            stock2 = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2",
                Open = 6,
                High = 7,
                Low = 8,
                Close = 9,
                Volume = 10000
            };
            Assert.IsFalse(stock1.Equals(stock2));
        }
    }
}
