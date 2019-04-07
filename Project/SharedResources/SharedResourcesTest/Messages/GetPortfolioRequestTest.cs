using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.Messages;
using Shared.PortfolioResources;
using Shared;
using System.Collections.Generic;

namespace SharedTest.Messages
{
    [TestClass]
    public class GetPortfolioRequestTest
    {
        [TestMethod]
        public void DefaultConstructorTest()
        {
            var newGetPortfolioRequest = new GetPortfolioRequest();

            Assert.IsNull(newGetPortfolioRequest.Account);
        }

        [TestMethod]
        public void InheritsMessageTest()
        {
            var newGetPortfolioRequest = new GetPortfolioRequest();

            Assert.AreEqual(0, newGetPortfolioRequest.SourceID);
            Assert.IsNull(newGetPortfolioRequest.ConversationID);
            Assert.IsNull(newGetPortfolioRequest.MessageID);
        }

        [TestMethod]
        public void InitializeTest()
        {
            var acc = new Shared.PortfolioResources.Portfolio();

            var m = new GetPortfolioRequest
            {
                Account = acc
            };

            Assert.AreEqual(m.Account.Assets.Count, 0);
            Assert.IsNull(m.Account.Password);
        }

        [TestMethod]
        public void SerilaizeTest()
        {
            var assets = new Dictionary<string, Asset>
            {
                { "CASH", new Asset() }
            };

            var acc = new Shared.PortfolioResources.Portfolio
            {
                Assets = assets,
                Username = "foohooboo",
                Password = "yolo",
                WriteAuthority = true,
                PortfolioID = 1
            };

            var m = new GetPortfolioRequest
            {
                Account = acc,
            };

            var serializedMessage = MessageFactory.GetMessage(m.Encode(), false) as GetPortfolioRequest;

            Assert.IsNotNull(serializedMessage);
            Assert.AreEqual(serializedMessage.Account.Assets.Count, 1);
            Assert.AreEqual(serializedMessage.Account.Password, "yolo");
            Assert.AreEqual(serializedMessage.Account.Username, "foohooboo");
            Assert.AreEqual(serializedMessage.Account.PortfolioID, 1);
            Assert.IsTrue(serializedMessage.Account.WriteAuthority);
        }
    }
}
