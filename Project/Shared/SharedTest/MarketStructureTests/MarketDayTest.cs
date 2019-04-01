using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.MarketStructures;
using System.Linq;

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

            Assert.IsNull(MarketDay.TradedCompanies);
            Assert.IsNull(MarketDay.Date);
        }

        [TestMethod]
        public void DateConstructorTest()
        {
            string date = "1990-02-20";
            var MarketDay = new MarketDay(date);

            Assert.AreEqual(1,MarketDay.TradedCompanies.Count);
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

            Assert.AreEqual(3, MarketDay.TradedCompanies.Count);
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

            MarketDay.TradedCompanies = stocks.Cast<ValuatedStock>().ToList();

            Assert.AreEqual(3, MarketDay.TradedCompanies.Count);
            Assert.AreEqual(MarketDay.Date, date);
        }
    }
}
