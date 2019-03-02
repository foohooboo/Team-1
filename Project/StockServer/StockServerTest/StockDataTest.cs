using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockServer.Data;
using System.Collections.Generic;
using Shared.MarketStructures;

namespace StockServerTest
{

    //TODO: Add more tests. I added this as a very basic START to the test project.
    //Dsphar 2/27/2019

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
            var numCompanies = marketDay.Count;

            for(int i=0; i<numDays; i++)
            {
                Assert.AreEqual("Apple Inc.", marketDay[0].Name);
                Assert.AreEqual("AAPL", marketDay[0].Symbol);
                Assert.AreEqual("Amazon.com Inc", marketDay[1].Name);
                Assert.AreEqual("AMZN", marketDay[1].Symbol);
                Assert.AreEqual(numCompanies, marketDay.Count);
                marketDay = StockData.AdvanceDay();
            }
        }
    }
}
