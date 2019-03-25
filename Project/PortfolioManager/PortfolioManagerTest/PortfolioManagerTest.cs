using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioManager;
using Shared.Portfolio;

namespace PortfolioManagerTest
{
    [TestClass]
    public class PortfolioManagerTest
    {
        private class TestManager : Manager
        {
            public TestManager() : base()
            {

            }

            public ConcurrentDictionary<int, Portfolio> GetPortfolios => portfolios;

            public new Portfolio GetNewPortfolio(string username, string password)
            {
                return base.GetNewPortfolio(username, password);
            }

            public new bool TryToAdd(Portfolio portfolio)
            {
                return base.TryToAdd(portfolio);
            }
        }

        [TestMethod]
        public void ConstructorTest()
        {
            var manager = new TestManager();

            Assert.IsNotNull(manager.GetPortfolios);
        }

        [TestMethod]
        public void AddPortfolioFailTest()
        {
        }

        [TestMethod]
        public void AddPortfolioSuccessTest()
        {
            var manager = new TestManager();
            var localPortfolio = manager.GetNewPortfolio(GetRandomString(3), GetRandomString(6));
            manager.TryToAdd(localPortfolio);

            Assert.IsTrue(manager.TryToGet(localPortfolio.PortfolioID, out Portfolio portfolio));
            Assert.AreSame(localPortfolio, portfolio);
        }

        [TestMethod]
        public void GetPortfolioFailTest()
        {
            var manager = new Manager();
            var portfolioID = 1;

            Assert.IsFalse(manager.TryToGet(portfolioID, out Portfolio portfolio));
            Assert.IsNull(portfolio);
        }

        [TestMethod]
        public void GetPortfolioSuccessTest()
        {
            var manager = new TestManager();
            var portfolio1 = manager.GetNewPortfolio(GetRandomString(3), GetRandomString(6));
            manager.TryToAdd(portfolio1);
            var portfolio2 = manager.GetNewPortfolio(GetRandomString(3), GetRandomString(6));
            manager.TryToAdd(portfolio2);
            var portfolio3 = manager.GetNewPortfolio(GetRandomString(3), GetRandomString(6));
            manager.TryToAdd(portfolio3);

            Assert.IsTrue(manager.TryToGet(portfolio2.PortfolioID, out Portfolio portfolio));
            Assert.AreSame(portfolio2, portfolio);
        }

        [TestMethod]
        public void UpdatePortfolioFailTest()
        {

        }

        [TestMethod]
        public void UpdatePortfolioSuccessTest()
        {

        }

        private static Random rand = new Random();

        private string GetRandomString(int length)
        {
            var word = String.Empty;
            var letters = @"abcdefghijklmnopqrstuvwxyz";

            for (int characterIndex = 1; characterIndex <= length; characterIndex++)
            {
                int letterIndex = rand.Next(0, letters.Length - 1);
                word += letters[letterIndex];
            }

            return word;
        }
    }
}