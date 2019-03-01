using Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.MarketStructureTests
{
    [TestClass]
    public class EvaluatedStockTest
    {
        public EvaluatedStockTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var evaluatedStock = new EvaluatedStock();

            Assert.AreEqual(evaluatedStock.close, 0.0F);
            Assert.AreEqual(evaluatedStock.high, 0.0F);
            Assert.AreEqual(evaluatedStock.low, 0.0F);
            Assert.AreEqual(evaluatedStock.open, 0.0F);
            Assert.AreEqual(evaluatedStock.volume, 0);
            Assert.IsNull(evaluatedStock.stock.Name);
            Assert.IsNull(evaluatedStock.stock.Symbol);
        }

        [TestMethod]
        public void LoadedConstructorTest()
        {
            var stock = new Stock("GOOGL","GOOGLE");
            string[] data = { "yolo", "1.0", "1.1", "1.2", "1.3", "2" }; 
            var evaluatedStock = new EvaluatedStock(data, stock);

            Assert.AreEqual(evaluatedStock.open, 1.0F);
            Assert.AreEqual(evaluatedStock.high, 1.1F);
            Assert.AreEqual(evaluatedStock.low, 1.2F);
            Assert.AreEqual(evaluatedStock.close, 1.3F);
            Assert.AreEqual(evaluatedStock.volume, 2);
            Assert.AreEqual(evaluatedStock.stock.Name, "GOOGLE");
            Assert.AreEqual(evaluatedStock.stock.Symbol, "GOOGL");
        }

        [TestMethod]
        public void InitializerTest()
        {
            var evaluatedStock = new EvaluatedStock
            {
                stock = new Stock("GOOGL", "GOOGLE"),
                open = 1.0F,
                high = 1.1F,
                low = 1.2F,
                close = 1.3F,
                volume = 2
            };

            Assert.AreEqual(evaluatedStock.open, 1.0F);
            Assert.AreEqual(evaluatedStock.high, 1.1F);
            Assert.AreEqual(evaluatedStock.low, 1.2F);
            Assert.AreEqual(evaluatedStock.close, 1.3F);
            Assert.AreEqual(evaluatedStock.volume, 2);
            Assert.AreEqual(evaluatedStock.stock.Name, "GOOGLE");
            Assert.AreEqual(evaluatedStock.stock.Symbol, "GOOGL");
        }
    }
}
