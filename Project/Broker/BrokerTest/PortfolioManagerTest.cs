using System.IO;
using Broker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.PortfolioResources;
using SharedResources.DataGeneration;

namespace BrokerTest
{
    [TestClass]
    [DoNotParallelize]
    public class PortfolioManagerTest
    {
        [TestMethod]
        [Priority(1)]
        public void SavePortfolioTest()
        {
            PortfolioManager.Clear();
            Assert.IsTrue(PortfolioManager.TryToCreate(DataGenerator.GetRandomString(5), DataGenerator.GetRandomString(6), out Portfolio portfolio));
            Assert.IsTrue(PortfolioManager.TryToCreate(DataGenerator.GetRandomString(5), DataGenerator.GetRandomString(6), out Portfolio portfolio2));
            Assert.IsTrue(PortfolioManager.TryToCreate(DataGenerator.GetRandomString(5), DataGenerator.GetRandomString(6), out Portfolio portfolio3));

            PortfolioManager.SavePortfolios();

            Assert.AreEqual(0, PortfolioManager.PortfolioCount);
            Assert.IsTrue(File.Exists(PortfolioManager.PortfolioData));

            PortfolioManager.Clear();
        }

        [TestMethod]
        [Priority(1)]
        public void LoadPortfolioTest()
        {
            File.Delete(PortfolioManager.PortfolioData);
            PortfolioManager.Clear();
            Assert.IsTrue(PortfolioManager.TryToCreate(DataGenerator.GetRandomString(5), DataGenerator.GetRandomString(6), out Portfolio portfolio1));
            Assert.IsTrue(PortfolioManager.TryToCreate(DataGenerator.GetRandomString(5), DataGenerator.GetRandomString(6), out Portfolio portfolio2));
            Assert.IsTrue(PortfolioManager.TryToCreate(DataGenerator.GetRandomString(5), DataGenerator.GetRandomString(6), out Portfolio portfolio3));

            PortfolioManager.SavePortfolios();
            PortfolioManager.LoadPortfolios();
            Assert.IsTrue(PortfolioManager.TryToGet(portfolio1.PortfolioID, out Portfolio p1));
            Assert.IsTrue(PortfolioManager.TryToGet(portfolio2.PortfolioID, out Portfolio p2));
            Assert.IsTrue(PortfolioManager.TryToGet(portfolio3.PortfolioID, out Portfolio p3));
            Assert.IsTrue(p1.Equals(portfolio1));
            Assert.IsTrue(p2.Equals(portfolio2));
            Assert.IsTrue(p3.Equals(portfolio3));

            PortfolioManager.TryToRemove( portfolio1.PortfolioID);
            PortfolioManager.TryToRemove(portfolio2.PortfolioID);
            PortfolioManager.TryToRemove(portfolio3.PortfolioID);
        }

        [TestMethod]
        public void AddPortfolioSuccessTest()
        {
            Assert.IsTrue(PortfolioManager.TryToCreate(DataGenerator.GetRandomString(3), DataGenerator.GetRandomString(6), out Portfolio portfolio));

            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }

        [TestMethod]
        public void AddPortfolioFailTest()
        {
            PortfolioManager.TryToCreate(DataGenerator.GetRandomString(3), DataGenerator.GetRandomString(6), out Portfolio portfolio);

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

            Assert.IsTrue(PortfolioManager.TryToGet(id, out Portfolio newPullPortfolio));
            Assert.AreEqual(id, newPullPortfolio.PortfolioID);
            Assert.AreEqual(username, newPullPortfolio.Username);
            Assert.AreEqual(password, newPullPortfolio.Password);

            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }

        [TestMethod]
        public void GetPortfolioByCredentialsSuccessTest()
        {
            PortfolioManager.TryToCreate(DataGenerator.GetRandomString(5), DataGenerator.GetRandomString(2), out Portfolio portfolio);

            var id = portfolio.PortfolioID;
            var username = portfolio.Username;
            var password = portfolio.Password;

            Assert.IsTrue(PortfolioManager.TryToGet(username, password, out Portfolio newPullPortfolio));
            Assert.AreEqual(id, newPullPortfolio.PortfolioID);
            Assert.AreEqual(username, newPullPortfolio.Username);
            Assert.AreEqual(password, newPullPortfolio.Password);

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

            Assert.IsFalse(PortfolioManager.TryToRemove(portfolio.PortfolioID + 123));
            Assert.AreEqual(count, PortfolioManager.Portfolios.Count);

            PortfolioManager.TryToRemove(portfolio.PortfolioID);
        }

        [TestMethod]
        public void RemovePortfolioSuccessTest()
        {
            PortfolioManager.TryToCreate(DataGenerator.GetRandomString(3), DataGenerator.GetRandomString(6), out Portfolio portfolio);
            var count = PortfolioManager.Portfolios.Count;
            var id = portfolio.PortfolioID;

            Assert.IsTrue(PortfolioManager.TryToRemove(id));
            Assert.AreEqual(count - 1, PortfolioManager.Portfolios.Count);
        }
    }
}