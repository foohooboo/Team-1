﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.MarketStructures;
using Shared.PortfolioResources;

namespace SharedTest.PortfolioTests
{
    [TestClass]
    public class AssetTest
    {
        [TestMethod]
        public void GetTestAssetTest()
        {
            var asset = Asset.GetTestAsset();

            Assert.IsNotNull(asset.RelatedStock);
            Assert.AreEqual(500, asset.Quantity);
        }

        [TestMethod]
        public void TestConstructor()
        {
            var ass = new Asset();

            Assert.AreEqual(ass.Quantity, 0);
            Assert.IsNull(ass.RelatedStock.Name);
        }

        [TestMethod]
        public void TestInitializer()
        {
            var testStock = new Stock("CASH", "MONEY");

            var asset = new Asset
            {
                Quantity = 10,
                RelatedStock = testStock
            };

            Assert.IsNotNull(asset);
            Assert.AreEqual(10, asset.Quantity);
            Assert.AreEqual(asset.RelatedStock, testStock);
        }
    }
}
