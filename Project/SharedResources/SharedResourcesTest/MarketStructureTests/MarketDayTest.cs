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

        [TestMethod]
        public void EqualsOverrideTest()
        {
            //Prepare day
            var stock1 = new ValuatedStock()
            {
                Symbol = "STK1",
                Name = "Stock 1",
                Open = 1,
                High = 2,
                Low = 3,
                Close = 4,
                Volume = 5
            };
            var stock2 = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2",
                Open = 6,
                High = 7,
                Low = 8,
                Close = 9,
                Volume = 10
            };
            ValuatedStock[] stocks = { stock1, stock2 };
            var marketDay = new MarketDay("testDay", stocks);

            //Prepare day duplicate
            var stock1Dup = new ValuatedStock()
            {
                Symbol = "STK1",
                Name = "Stock 1",
                Open = 1,
                High = 2,
                Low = 3,
                Close = 4,
                Volume = 5
            };
            var stock2Dup = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2",
                Open = 6,
                High = 7,
                Low = 8,
                Close = 9,
                Volume = 10
            };
            ValuatedStock[] stocksDup = { stock1Dup, stock2Dup };
            var marketDayDup = new MarketDay("testDay", stocksDup);

            //Equal
            Assert.IsTrue(marketDay.Equals(marketDayDup));

            //Different date
            marketDayDup = new MarketDay("testDifferentDay", stocksDup);
            Assert.IsFalse(marketDay.Equals(marketDayDup));

            //One missing stock
            stocksDup = new ValuatedStock[] { stock1Dup };
            marketDayDup = new MarketDay("testDay", stocksDup);
            Assert.IsFalse(marketDay.Equals(marketDayDup));

            //One Extra Stock
            var stock3 = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2",
                Open = 60,
                High = 70,
                Low = 80,
                Close = 90,
                Volume = 100
            };
            stocksDup = new ValuatedStock[] { stock1Dup, stock2Dup, stock3 };
            marketDayDup = new MarketDay("testDay", stocksDup);
            Assert.IsFalse(marketDay.Equals(marketDayDup));
                       
        }
    }
}
