using Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.MarketStructureTests
{
    [TestClass]
    public class EvaluatedStocksTest
    {
        public EvaluatedStocksTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var evaluatedStocks = new EvaluatedStocks();

            Assert.AreEqual(evaluatedStocks.Count, 0);
            Assert.IsNull(evaluatedStocks.Date);
        }

        [TestMethod]
        public void DateConstructorTest()
        {
            string date = "1990-02-20";
            var evaluatedStocks = new EvaluatedStocks(date);

            Assert.AreEqual(evaluatedStocks.Count, 0);
            Assert.AreEqual(evaluatedStocks.Date, date);
        }

        [TestMethod]
        public void LoadedConstructorTest()
        {
            string date = "1990-02-20";
            var stock1 = new EvaluatedStock();
            var stock2 = new EvaluatedStock();

            EvaluatedStock[] stocks = { stock1, stock2 };
            
            var evaluatedStocks = new EvaluatedStocks(date, stocks);

            Assert.AreEqual(evaluatedStocks.Count, 2);
            Assert.AreEqual(evaluatedStocks.Date, date);
        }

        [TestMethod]
        public void InitializerTest()
        {
            string date = "1990-02-20";
            var stock1 = new EvaluatedStock();
            var stock2 = new EvaluatedStock();

            EvaluatedStock[] stocks = { stock1, stock2 };

            var evaluatedStocks = new EvaluatedStocks
            {
                Date = date
            };

            evaluatedStocks.AddRange(stocks);

            Assert.AreEqual(evaluatedStocks.Count, 2);
            Assert.AreEqual(evaluatedStocks.Date, date);
        }
    }
}
