using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.Messages;
using Shared.Portfolio;
using Shared;
using System.Collections.Generic;

namespace SharedTest.Messages
{
    [TestClass]
    public class PortfolioUpdateMessageTest
    {
        public PortfolioUpdateMessageTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var updateMessage = new PortfolioUpdateMessage();

            Assert.AreEqual(updateMessage.PortfolioID, 0);
            Assert.IsFalse(updateMessage.RequestWriteAuthority);
            Assert.AreEqual(updateMessage.Assets.Count, 0);
        }

        [TestMethod]
        public void AssetConstructorTest()
        {
            var ass1 = new Asset
            {
                Quantity = 1,
                RelatedStock = new Stock("GOOGL", "GOOGLE")
            };
            var assets = new Dictionary<string, Asset>();
            assets.Add(ass1.RelatedStock.Symbol, ass1);

            var updateMessage = new PortfolioUpdateMessage(assets);

            Assert.AreEqual(updateMessage.PortfolioID, 0);
            Assert.IsFalse(updateMessage.RequestWriteAuthority);
            Assert.AreEqual(updateMessage.Assets.Count, 1);
        }

        [TestMethod]
        public void InitializeTest()
        {
            var ass1 = new Asset
            {
                Quantity = 1,
                RelatedStock = new Stock("GOOGL", "GOOGLE")
            };
            var assets = new Dictionary<string, Asset>();
            assets.Add(ass1.RelatedStock.Symbol, ass1);

            var updateMessage = new PortfolioUpdateMessage
            {
                Assets = assets,
                PortfolioID = 1,
                RequestWriteAuthority = true
            };

            Assert.AreEqual(updateMessage.PortfolioID, 1);
            Assert.IsTrue(updateMessage.RequestWriteAuthority);
            Assert.AreEqual(updateMessage.Assets.Count, 1);
        }
    }
}
