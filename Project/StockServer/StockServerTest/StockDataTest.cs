using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockServer.Data;

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
            var stockData = new StockData();

            Assert.AreSame(stockData.Data[0][0].stock.Name, "Apple Inc.");
            Assert.AreSame(stockData.Data[0][0].stock.Symbol, "AAPL");
        }
    }
}
