using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.Messages;

namespace SharedTest.Messages
{
    [TestClass]
    public class CreatePortfolioRequestMessageTest
    {
        public CreatePortfolioRequestMessageTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var newCreatePortfolioRequest = new CreatePortfolioRequestMessage();

            Assert.IsNull(newCreatePortfolioRequest.ConfirmPassword);
        }

        [TestMethod]
        public void InheritsGetPortfolioRequestTest()
        {
            var newCreatePortfolioRequest = new CreatePortfolioRequestMessage();

            Assert.IsNull(newCreatePortfolioRequest.Account);
        }

        [TestMethod]
        public void TestInitialize()
        {
            string password = "yolo";

            var newCreatePortfolioRequest = new CreatePortfolioRequestMessage
            {
                ConfirmPassword = password
            };
        }

        [TestMethod]
        public void SerializeTest()
        {
            string password = "yolo";

            var newCreatePortfolioRequest = new CreatePortfolioRequestMessage
            {
                ConfirmPassword = password
            };

            var serializedMessage = MessageFactory.GetMessage(newCreatePortfolioRequest.Encode(), false) as CreatePortfolioRequestMessage;

            Assert.AreEqual(newCreatePortfolioRequest.ConfirmPassword, serializedMessage.ConfirmPassword);
        }
    }
}
