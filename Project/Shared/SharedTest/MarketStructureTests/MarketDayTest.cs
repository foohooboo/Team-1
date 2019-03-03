using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.MarketStructures;

namespace SharedTest.MarketStructureTests
{
    [TestClass]
    public class MarketDayTest
    {
        public MarketDayTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var MarketDay = new MarketDay();

            Assert.AreEqual(MarketDay.Data.Count, 0);
            Assert.IsNull(MarketDay.Date);
        }

        [TestMethod]
        public void DateConstructorTest()
        {
            string date = "1990-02-20";
            var MarketDay = new MarketDay(date);

            Assert.AreEqual(MarketDay.Data.Count, 0);
            Assert.AreEqual(MarketDay.Date, date);
        }

        [TestMethod]
        public void LoadedConstructorTest()
        {
            string date = "1990-02-20";
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();

            ValuatedStock[] stocks = { stock1, stock2 };
            
            var MarketDay = new MarketDay(date, stocks);

            Assert.AreEqual(MarketDay.Data.Count, 2);
            Assert.AreEqual(MarketDay.Date, date);
        }

        [TestMethod]
        public void InitializerTest()
        {
            string date = "1990-02-20";
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();

            ValuatedStock[] stocks = { stock1, stock2 };

            var MarketDay = new MarketDay
            {
                Date = date
            };

            MarketDay.Data.AddRange(stocks);

            Assert.AreEqual(MarketDay.Data.Count, 2);
            Assert.AreEqual(MarketDay.Date, date);
        }
    }
}
