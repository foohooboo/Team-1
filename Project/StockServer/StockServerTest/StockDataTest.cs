using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockServer.Data;
using System.Collections.Generic;

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
            List<Shared.EvaluatedStocks> stockData = new StockData().Data;
            //How this datatype Works
            //stockData = new List<EvaluatedStocks>();
            //EvaluatedStocks: List<EvaluatedStock>
            //EvaluatedStock
            Assert.AreSame(stockData[0][0].stock.Name, "Apple Inc.");
            Assert.AreSame(stockData[0][0].stock.Symbol, "AAPL");
            Assert.AreSame(stockData.Count, 1000);
            int stockCount = stockData[0].Count;
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreSame(stockData[i].Count, stockCount);
            }
        }
    }
}
