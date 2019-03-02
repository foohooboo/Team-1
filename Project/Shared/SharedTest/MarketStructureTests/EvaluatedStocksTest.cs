using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.MarketStructures;

namespace SharedTest.MarketStructureTests
{
    [TestClass]
    public class StockMarketDayTest
    {
        public StockMarketDayTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var StockMarketDay = new StockMarketDay();

            Assert.AreEqual(StockMarketDay.Count, 0);
            Assert.IsNull(StockMarketDay.Date);
        }

        [TestMethod]
        public void DateConstructorTest()
        {
            string date = "1990-02-20";
            var StockMarketDay = new StockMarketDay(date);

            Assert.AreEqual(StockMarketDay.Count, 0);
            Assert.AreEqual(StockMarketDay.Date, date);
        }

        [TestMethod]
        public void LoadedConstructorTest()
        {
            string date = "1990-02-20";
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();

            ValuatedStock[] stocks = { stock1, stock2 };
            
            var StockMarketDay = new StockMarketDay(date, stocks);

            Assert.AreEqual(StockMarketDay.Count, 2);
            Assert.AreEqual(StockMarketDay.Date, date);
        }

        [TestMethod]
        public void InitializerTest()
        {
            string date = "1990-02-20";
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();

            ValuatedStock[] stocks = { stock1, stock2 };

            var StockMarketDay = new StockMarketDay
            {
                Date = date
            };

            StockMarketDay.AddRange(stocks);

            Assert.AreEqual(StockMarketDay.Count, 2);
            Assert.AreEqual(StockMarketDay.Date, date);
        }
    }
}
