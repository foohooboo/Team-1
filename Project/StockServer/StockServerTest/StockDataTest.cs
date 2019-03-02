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
        public void LoadDataTest()
        {
            List<MarketDay> stockData = new StockData().Data;
            //How this datatype Works
            //stockData = new List<EvaluatedStocks>();
            //EvaluatedStocks: List<EvaluatedStock>
            //EvaluatedStock
            Assert.AreEqual(stockData[0][0].Name, "Apple Inc.");
            Assert.AreEqual(stockData[0][0].Symbol, "AAPL");
            Assert.AreEqual(stockData.Count, 1000);

            int numCompanies = stockData[0].Count;
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(stockData[i].Count, numCompanies);
            }
        }
    }
}
