using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.Messages;

namespace SharedTest.Messages
{
    [TestClass]
    public class ErrorTest
    {
        [TestMethod]
        public void DefaultConstructorTest()
        {
            var newError = new ErrorMessage();

            Assert.IsNull(newError.ErrorText);
        }

        [TestMethod]
        public void InitializeTest()
        {
            string err = "error";

            var m = new ErrorMessage
            {
                ErrorText = err
            };

            Assert.AreEqual(err, m.ErrorText);
        }

        [TestMethod]
        public void SerializeTest()
        {
            string err = "error";

            var m = new ErrorMessage
            {
                ErrorText = err
            };

            var serailizedMessage = MessageFactory.GetMessage(m.Encode()) as ErrorMessage;

            Assert.AreEqual(m.ErrorText, serailizedMessage.ErrorText);
        }
    }
}
