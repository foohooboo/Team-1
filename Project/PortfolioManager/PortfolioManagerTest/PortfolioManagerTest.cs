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
            var manager = new Manager();
            var localPortfolio = GetPortfolio(false);
            manager.TryToAdd(localPortfolio, out string error);

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
            var manager = new Manager();
            var portfolio1 = GetPortfolio(false);
            var portfolio2 = GetPortfolio(false);
            var portfolio3 = GetPortfolio(false);
            manager.TryToAdd(portfolio1, out string error1);
            manager.TryToAdd(portfolio2, out string error2);
            manager.TryToAdd(portfolio3, out string error3);

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

        private Portfolio GetPortfolio(bool writeAuthority)
        {
            var account = new Portfolio
            {
                Username = GetRandomString(5),
                Password = GetRandomString(4),
                RequestWriteAuthority = writeAuthority
            };

            return account;
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