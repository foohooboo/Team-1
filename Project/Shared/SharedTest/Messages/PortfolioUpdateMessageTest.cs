using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.Messages;
using Shared.Portfolio;
using System.Collections.Generic;
using Shared.MarketStructures;

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
            Assert.IsFalse(updateMessage.WriteAuthority);
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
            var assets = new Dictionary<string, Asset>
            {
                { ass1.RelatedStock.Symbol, ass1 }
            };

            var updateMessage = new PortfolioUpdateMessage(assets);

            Assert.AreEqual(updateMessage.PortfolioID, 0);
            Assert.IsFalse(updateMessage.WriteAuthority);
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
            var assets = new Dictionary<string, Asset>
            {
                { ass1.RelatedStock.Symbol, ass1 }
            };

            var updateMessage = new PortfolioUpdateMessage
            {
                Assets = assets,
                PortfolioID = 1,
                WriteAuthority = true
            };

            Assert.AreEqual(updateMessage.PortfolioID, 1);
            Assert.IsTrue(updateMessage.WriteAuthority);
            Assert.AreEqual(updateMessage.Assets.Count, 1);
        }

        [TestMethod]
        public void SerializeTest()
        {
            var ass1 = new Asset
            {
                Quantity = 1,
                RelatedStock = new Stock("GOOGL", "GOOGLE")
            };
            var assets = new Dictionary<string, Asset>
            {
                { ass1.RelatedStock.Symbol, ass1 }
            };

            var updateMessage = new PortfolioUpdateMessage
            {
                Assets = assets,
                PortfolioID = 1,
                WriteAuthority = true
            };

            var serializedMessage = MessageFactory.GetMessage(updateMessage.Encode(), false) as PortfolioUpdateMessage;

            Assert.AreEqual(updateMessage.PortfolioID, serializedMessage.PortfolioID);
            Assert.AreEqual(updateMessage.WriteAuthority, serializedMessage.WriteAuthority);
            Assert.AreEqual(updateMessage.Assets.Count, serializedMessage.Assets.Count);
            Assert.AreEqual(updateMessage.Assets.ContainsKey("GOOGL"), serializedMessage.Assets.ContainsKey("GOOGL"));
            Assert.AreEqual(updateMessage.Assets["GOOGL"].Quantity, serializedMessage.Assets["GOOGL"].Quantity);
            Assert.AreEqual(updateMessage.Assets["GOOGL"].RelatedStock.Name, serializedMessage.Assets["GOOGL"].RelatedStock.Name);
        }
    }
}
