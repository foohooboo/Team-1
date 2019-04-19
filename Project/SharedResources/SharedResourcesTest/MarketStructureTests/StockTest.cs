using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.MarketStructures;

namespace SharedTest.MarketStructureTests
{
    [TestClass]
    public class StockTest
    {
        public StockTest()
        {

        }

        [TestMethod]
        public void GetTestStockTest()
        {
            var stock = Stock.GetTestStock();
            Assert.AreEqual(stock.Symbol.Length, 3);
            Assert.AreEqual(stock.Name.Length, 7);
        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var stock = new Stock();

            Assert.IsNull(stock.Symbol);
            Assert.IsNull(stock.Name);
        }

        [TestMethod]
        public void LoadedConstructorTest()
        {
            string symbol = "GOOGL";
            string name = "GOOGLE";

            var stock = new Stock(symbol, name);

            Assert.AreEqual(stock.Symbol, symbol);
            Assert.AreEqual(stock.Name, name);
        }

        [TestMethod]
        public void InitializerTest()
        {
            string symbol = "GOOGL";
            string name = "GOOGLE";

            var stock = new Stock
            {
                Symbol = symbol,
                Name = name
            };

            Assert.AreEqual(stock.Symbol, symbol);
            Assert.AreEqual(stock.Name, name);
        }
    }
}
