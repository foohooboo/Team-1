using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Portfolio;
using Shared;

namespace SharedTest.PortfolioTests
{
    [TestClass]
    public class AssetTest
    {
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

            var ass = new Asset
            {
                Quantity = 10,
                RelatedStock = testStock
            };

            Assert.IsNotNull(ass);
            Assert.AreEqual(10, ass.Quantity);
            Assert.AreEqual(ass.RelatedStock, testStock);
        }
    }
}
