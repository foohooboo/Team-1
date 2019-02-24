using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.Messages;

namespace SharedTest.MessageTests
{
    [TestClass]
    public class MessageTest
    {
        /// <summary>
        /// This class allows the abstract class to be tested.
        /// </summary>
        public class TestMessage : Message
        {
            public TestMessage()
            {

            }
        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var newMessage = new TestMessage();

            Assert.AreEqual(0, newMessage.SourceID);
            Assert.IsNull(newMessage.ConversationID);
            Assert.IsNull(newMessage.MessageID);

            var sid = 1;
            var cid = "3-1-3";
            var mid = "4-2-3";

            var m = new TestMessage
            {
                SourceID = sid,
                ConversationID = cid,
                MessageID = mid
            };

            Assert.AreEqual(sid, m.SourceID);
            Assert.AreEqual(cid, m.ConversationID);
            Assert.AreEqual(mid, m.MessageID);
        }

        [TestMethod]
        public void InitializeTest()
        {
            var sid = 1;
            var cid = "3-1-3";
            var mid = "4-2-3";

            var m = new TestMessage
            {
                SourceID = sid,
                ConversationID = cid,
                MessageID = mid
            };

            Assert.AreEqual(sid, m.SourceID);
            Assert.AreEqual(cid, m.ConversationID);
            Assert.AreEqual(mid, m.MessageID);
        }

        [TestMethod]
        public void SerilaizeTest()
        {
            var sid = 1;
            var cid = "3-1-3";
            var mid = "4-2-3";

            var m = new TestMessage
            {
                SourceID = sid,
                ConversationID = cid,
                MessageID = mid
            };

            var serailizedMessage = MessageFactory.GetMessage(m.Encode());

            Assert.AreEqual(serailizedMessage.SourceID, m.SourceID);
            Assert.AreEqual(serailizedMessage.ConversationID, m.ConversationID);
            Assert.AreEqual(serailizedMessage.MessageID, m.MessageID);
        }
    }
}