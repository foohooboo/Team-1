using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.MarketStructures;
using Shared.PortfolioResources;

namespace SharedTest.PortfolioTests
{
    [TestClass]
    public class PortfolioTest
    {
        public PortfolioTest()
        {

        }

        [TestMethod]
        public void GetTestPortfolioTeset()
        {
            var portfolio = Portfolio.GetTestPortfolio();

            Assert.IsNotNull(portfolio.PortfolioID);
            Assert.IsNotNull(portfolio.Username);
            Assert.IsNotNull(portfolio.Password);
            Assert.AreEqual(portfolio.Assets.Count, 3);

        }

        [TestMethod]
        public void TestConstructor()
        {
            var account = new Portfolio();

            Assert.AreEqual(account.Assets.Count, 0);
            Assert.AreEqual(account.PortfolioID, 0);
            Assert.IsNull(account.Password);
            Assert.IsNull(account.Username);
            Assert.IsFalse(account.WriteAuthority);
        }

        [TestMethod]
        public void TestInitializer()
        {
            var assets = new Dictionary<string, Asset>();

            var stock1 = new Stock("CASH", "CASH");
            var ass1 = new Asset
            {
                RelatedStock = stock1,
                Quantity = 1
            };

            var stock2 = new Stock("MONEY", "MONEY");
            var ass2 = new Asset
            {
                RelatedStock = stock2,
                Quantity = 2
            };

            assets.Add(ass1.RelatedStock.Symbol, ass1);
            assets.Add(ass2.RelatedStock.Symbol, ass2);

            var account = new Portfolio
            {
                Assets = assets,
                Username = "foohooboo",
                Password = "yolo",
                WriteAuthority = true,
                PortfolioID = 1
            };

            Assert.AreEqual(account.Assets.Count, 2);
            Assert.AreEqual(account.PortfolioID, 1);
            Assert.AreEqual(account.Username, "foohooboo");
            Assert.AreEqual(account.Password, "yolo");
            Assert.IsTrue(account.WriteAuthority);
        }

        [TestMethod]
        public void TestAddNewAsset()
        {
            var stock1 = new Stock("GOOGL", "GOOGLE");
            var stock2 = new Stock("CASH", "MONEY");

            var ass1 = new Asset
            {
                Quantity = 1,
                RelatedStock = stock1
            };
            var ass2 = new Asset
            {
                Quantity = 1,
                RelatedStock = stock2
            };

            var account = new Portfolio();

            account.ModifyAsset(ass1);

            Assert.AreEqual(account.Assets.Count, 1);
            Assert.IsTrue(account.Assets.ContainsKey("GOOGL"));
            Assert.AreEqual(account.Assets["GOOGL"].Quantity, 1);
            Assert.AreEqual(account.Assets["GOOGL"].RelatedStock.Symbol, "GOOGL");
            Assert.AreEqual(account.Assets["GOOGL"].RelatedStock.Name, "GOOGLE");
        }

        [TestMethod]
        public void TestIncrementAsset()
        {
            var stock1 = new Stock("GOOGL", "GOOGLE");
            var stock2 = new Stock("CASH", "MONEY");

            var ass1 = new Asset
            {
                Quantity = 1,
                RelatedStock = stock1
            };

            var account = new Portfolio();

            account.ModifyAsset(ass1);
            account.ModifyAsset(ass1);

            Assert.AreEqual(account.Assets.Count, 1);
            Assert.IsTrue(account.Assets.ContainsKey("GOOGL"));
            Assert.AreEqual(account.Assets["GOOGL"].Quantity, 2);
            Assert.AreEqual(account.Assets["GOOGL"].RelatedStock.Symbol, "GOOGL");
            Assert.AreEqual(account.Assets["GOOGL"].RelatedStock.Name, "GOOGLE");
        }

        [TestMethod]
        public void TestRemoveAsset()
        {
            var stock1 = new Stock("GOOGL", "GOOGLE");

            var ass1 = new Asset
            {
                Quantity = 1,
                RelatedStock = stock1
            };
            var ass2 = new Asset
            {
                Quantity = -1,
                RelatedStock = stock1
            };

            var account = new Portfolio();

            account.ModifyAsset(ass1);
            account.ModifyAsset(ass2);

            Assert.AreEqual(account.Assets.Count, 0);
            Assert.IsFalse(account.Assets.ContainsKey("GOOGL"));
        }

        [TestMethod]
        public void TestDecrementAsset()
        {
            var stock1 = new Stock("GOOGL", "GOOGLE");

            var ass1 = new Asset
            {
                Quantity = 2,
                RelatedStock = stock1
            };
            var ass2 = new Asset
            {
                Quantity = -1,
                RelatedStock = stock1
            };

            var account = new Portfolio();

            account.ModifyAsset(ass1);
            account.ModifyAsset(ass2);

            Assert.AreEqual(account.Assets.Count, 1);
            Assert.IsTrue(account.Assets.ContainsKey("GOOGL"));
            Assert.AreEqual(account.Assets["GOOGL"].Quantity, 1);
            Assert.AreEqual(account.Assets["GOOGL"].RelatedStock.Symbol, "GOOGL");
            Assert.AreEqual(account.Assets["GOOGL"].RelatedStock.Name, "GOOGLE");
        }

        [TestMethod]
        public void TestRemoveAssetOverflow()
        {
            var stock1 = new Stock("GOOGL", "GOOGLE");

            var ass1 = new Asset
            {
                Quantity = 1,
                RelatedStock = stock1
            };
            var ass2 = new Asset
            {
                Quantity = -10,
                RelatedStock = stock1
            };

            var account = new Portfolio();

            account.ModifyAsset(ass1);
            account.ModifyAsset(ass2);

            Assert.AreEqual(account.Assets.Count, 0);
            Assert.IsFalse(account.Assets.ContainsKey("GOOGL"));
        }
    }
}
