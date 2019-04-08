using System;
using Broker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.PortfolioResources;
using SharedResources.DataGeneration;

namespace BrokerTest
{
    [TestClass]
    public class PortfolioManagerTest
    {
        [TestMethod]
        public void AddPortfolioSuccessTest()
        {
            Assert.IsTrue(PortfolioManager.TryToCreate(DataGenerator.GetRandomString(3), DataGenerator.GetRandomString(6), out Portfolio portfolio));

            PortfolioManager.ReleaseLock(portfolio);
            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }

        [TestMethod]
        public void AddPortfolioFailTest()
        {
            PortfolioManager.TryToCreate(DataGenerator.GetRandomString(3), DataGenerator.GetRandomString(6), out Portfolio portfolio);
            PortfolioManager.ReleaseLock(portfolio);

            Assert.IsFalse(PortfolioManager.TryToCreate(portfolio.Username, DataGenerator.GetRandomString(6), out Portfolio newPortfolio));
            Assert.IsNull(newPortfolio);

            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }

        [TestMethod]
        public void GetPortfolioByIdSuccessTest()
        {
            PortfolioManager.TryToCreate(DataGenerator.GetRandomString(3), DataGenerator.GetRandomString(6), out Portfolio portfolio);

            var id = portfolio.PortfolioID;
            var username = portfolio.Username;
            var password = portfolio.Password;

            PortfolioManager.ReleaseLock(portfolio);

            Assert.IsTrue(PortfolioManager.TryToGet(id, out Portfolio newPullPortfolio));
            Assert.AreEqual(id, newPullPortfolio.PortfolioID);
            Assert.AreEqual(username, newPullPortfolio.Username);
            Assert.AreEqual(password, newPullPortfolio.Password);

            PortfolioManager.ReleaseLock(newPullPortfolio);
            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }

        [TestMethod]
        public void GetPortfolioByCredentialsSuccessTest()
        {
            PortfolioManager.TryToCreate(DataGenerator.GetRandomString(5), DataGenerator.GetRandomString(2), out Portfolio portfolio);

            var id = portfolio.PortfolioID;
            var username = portfolio.Username;
            var password = portfolio.Password;

            PortfolioManager.ReleaseLock(portfolio);

            Assert.IsTrue(PortfolioManager.TryToGet(username, password, out Portfolio newPullPortfolio));
            Assert.AreEqual(id, newPullPortfolio.PortfolioID);
            Assert.AreEqual(username, newPullPortfolio.Username);
            Assert.AreEqual(password, newPullPortfolio.Password);

            PortfolioManager.ReleaseLock(newPullPortfolio);
            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }

        [TestMethod]
        public void GetPortfolioFailTest()
        {
            Assert.IsFalse(PortfolioManager.TryToGet(PortfolioManager.Portfolios.Count + 1, out Portfolio portfolio));
            Assert.IsNull(portfolio);
        }

        [TestMethod]
        public void RemovePortfolioFailTest()
        {
            PortfolioManager.TryToCreate(DataGenerator.GetRandomString(3), DataGenerator.GetRandomString(6), out Portfolio portfolio);
            var count = PortfolioManager.Portfolios.Count;

            Assert.IsFalse(PortfolioManager.TryToRemove(portfolio.PortfolioID));
            Assert.AreEqual(count, PortfolioManager.Portfolios.Count);

            PortfolioManager.ReleaseLock(portfolio);
            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }

        [TestMethod]
        public void RemovePortfolioSuccessTest()
        {
            PortfolioManager.TryToCreate(DataGenerator.GetRandomString(3), DataGenerator.GetRandomString(6), out Portfolio portfolio);
            var count = PortfolioManager.Portfolios.Count;
            var id = portfolio.PortfolioID;
            PortfolioManager.ReleaseLock(portfolio);

            Assert.IsTrue(PortfolioManager.TryToRemove(id));
            Assert.AreEqual(count - 1, PortfolioManager.Portfolios.Count);
        }
    }
}