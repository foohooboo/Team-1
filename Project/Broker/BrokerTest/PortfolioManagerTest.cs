using System;
using Broker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Portfolio;

namespace BrokerTest
{
    [TestClass]
    public class PortfolioManagerTest
    {
        [TestMethod]
        public void AddPortfolioFailTest()
        {
        }

        [TestMethod]
        public void AddPortfolioSuccessTest()
        {
            var localPortfolio = PortfolioManager.GetNewPortfolio(GetRandomString(3), GetRandomString(6));
            PortfolioManager.TryToAdd(localPortfolio);

            Assert.IsTrue(PortfolioManager.TryToGet(localPortfolio.PortfolioID, out Portfolio portfolio));
            Assert.AreSame(localPortfolio, portfolio);
        }

        [TestMethod]
        public void GetPortfolioFailTest()
        {
            var portfolioID = 1;

            Assert.IsFalse(PortfolioManager.TryToGet(portfolioID, out Portfolio portfolio));
            Assert.IsNull(portfolio);
        }

        [TestMethod]
        public void GetPortfolioSuccessTest()
        {
            var portfolio1 = PortfolioManager.GetNewPortfolio(GetRandomString(3), GetRandomString(6));
            PortfolioManager.TryToAdd(portfolio1);
            var portfolio2 = PortfolioManager.GetNewPortfolio(GetRandomString(3), GetRandomString(6));
            PortfolioManager.TryToAdd(portfolio2);
            var portfolio3 = PortfolioManager.GetNewPortfolio(GetRandomString(3), GetRandomString(6));
            PortfolioManager.TryToAdd(portfolio3);

            Assert.IsTrue(PortfolioManager.TryToGet(portfolio2.PortfolioID, out Portfolio portfolio));
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