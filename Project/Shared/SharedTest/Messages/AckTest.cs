using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.Messages;

namespace SharedTest.Messages
{
    [TestClass]
    public class AckTest
    {
        [TestMethod]
        public void DefaultConstructorTest()
        {
            var newAck = new AckMessage();

            Assert.IsNull(newAck.ReferenceMessageID);
            Assert.IsNull(newAck.AckHello);
        }

        [TestMethod]
        public void InitializeTest()
        {
            string referenceID = "1-2-3";
            string hello = "hello";

            var m = new AckMessage
            {
                ReferenceMessageID = referenceID,
                AckHello = hello
            };

            Assert.AreEqual(referenceID, m.ReferenceMessageID);
            Assert.AreEqual(hello, m.AckHello);
        }

        [TestMethod]
        public void SerilaizeTest()
        {
            string referenceID = "1-2-3";
            string hello = "hello";

            var m = new AckMessage
            {
                ReferenceMessageID = referenceID,
                AckHello = hello
            };

            var serailizedMessage = MessageFactory.GetMessage(m.Encode()) as AckMessage;

            Assert.AreEqual(serailizedMessage.ReferenceMessageID, m.ReferenceMessageID);
            Assert.AreEqual(serailizedMessage.AckHello, m.AckHello);
        }
    }
}
