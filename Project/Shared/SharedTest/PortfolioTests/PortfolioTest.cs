using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Portfolio;
using Shared;
using System.Collections.Generic;

namespace SharedTest.PortfolioTests
{
    [TestClass]
    public class PortfolioTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            var account = new Portfolio();

            Assert.AreEqual(account.Assets.Count, 0);
            Assert.AreEqual(account.PortfolioID, 0);
            Assert.IsNull(account.Password);
            Assert.IsNull(account.Username);
            Assert.IsFalse(account.RequestWriteAuthority);
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
                RequestWriteAuthority = true,
                PortfolioID = 1
            };

            Assert.AreEqual(account.Assets.Count, 2);
            Assert.AreEqual(account.PortfolioID, 1);
            Assert.AreEqual(account.Username, "foohooboo");
            Assert.AreEqual(account.Password, "yolo");
            Assert.IsTrue(account.RequestWriteAuthority);
        }

        //TODO
        [TestMethod]
        public void TestModifyAsset()
        {

        }
    }
}
