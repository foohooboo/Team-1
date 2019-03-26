using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockServer.Data;
using Shared.MarketStructures;

namespace StockServerTest.Data
{

    [TestClass]
    public class StockDataTest
    {
        [TestMethod]
        public void AdvanceDayTest()
        {
            StockData.Init();
            Assert.AreEqual(StockData.CurrentDayNumber, 0);

            StockData.AdvanceDay();
            Assert.AreEqual(StockData.CurrentDayNumber, 1);

            StockData.AdvanceDay();
            Assert.AreEqual(StockData.CurrentDayNumber, 2);

            StockData.Init();
            Assert.AreEqual(StockData.CurrentDayNumber, 0);

            //ensure proper rollover
            for (int i=0; i < StockData.GetSize()-1; i++)
                StockData.AdvanceDay();
            Assert.AreEqual(StockData.GetSize() - 1, StockData.CurrentDayNumber);
            StockData.AdvanceDay();
            Assert.AreEqual(0, StockData.CurrentDayNumber);
        }

        [TestMethod]
        public void LoadDataTest()
        {
            StockData.Init();
            var marketDay = StockData.GetCurrentDay();
            var numDays = StockData.GetSize();
            var numCompanies = marketDay.TradedCompanies.Count;

            for (int i=0; i<numDays; i++)
            {
                Assert.AreEqual("Apple Inc.", marketDay.TradedCompanies[0].Name);
                Assert.AreEqual("AAPL", marketDay.TradedCompanies[0].Symbol);
                Assert.AreEqual("Amazon.com Inc", marketDay.TradedCompanies[1].Name);
                Assert.AreEqual("AMZN", marketDay.TradedCompanies[1].Symbol);
                Assert.AreEqual(numCompanies, marketDay.TradedCompanies.Count);
                marketDay = StockData.AdvanceDay();
            }
        }

        [TestMethod]
        public void GetRecentHistoryTest()
        {    
            //Setup control variables for verification
            var privateStockHistory = new PrivateObject(new StockData(), new PrivateType(typeof(StockData)));
            var entireHistory = privateStockHistory.Invoke("GetFullHistory") as MarketSegment;
            var firstDay = entireHistory[0];
            var secondDay = entireHistory[1];
            var thirdDay = entireHistory[2];
            var lastDay = entireHistory[entireHistory.Count - 1];

            //Reset StockData to day 0
            StockData.Init();

            //Current day zero, recentHistory should rollover.
            var recentHist = StockData.GetRecentHistory(2);
            Assert.AreEqual(lastDay, recentHist[0]);
            Assert.AreEqual(firstDay, recentHist[1]);

            //Current day 1, still a recentHistory rollover
            StockData.AdvanceDay();
            recentHist = StockData.GetRecentHistory(3);
            Assert.AreEqual(lastDay, recentHist[0]);
            Assert.AreEqual(firstDay, recentHist[1]);
            Assert.AreEqual(secondDay, recentHist[2]);

            //Current day 2, no more recentHistory rollover
            StockData.AdvanceDay();
            recentHist = StockData.GetRecentHistory(3);
            Assert.AreEqual(firstDay, recentHist[0]);
            Assert.AreEqual(secondDay, recentHist[1]);
            Assert.AreEqual(thirdDay, recentHist[2]);
        }
    }
}
