using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.Messages;

namespace SharedTest.Messages
{
    [TestClass]
    public class StockHistoryRequestMessageTest
    {
        public StockHistoryRequestMessageTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var streamRequestMessage = new StockHistoryRequestMessage();

            Assert.AreEqual(75, streamRequestMessage.TicksRequested);
        }

        [TestMethod]
        public void InitializerConstructorTest()
        {
            var streamRequestMessage = new StockHistoryRequestMessage
            {
                SourceID = 1,
                ConversationID = "3",
                MessageID = "1"
            };

            Assert.AreEqual(75, streamRequestMessage.TicksRequested);
            Assert.AreEqual(streamRequestMessage.SourceID, 1);
            Assert.AreEqual(streamRequestMessage.ConversationID, "3");
            Assert.AreEqual(streamRequestMessage.MessageID, "1");
        }

        [TestMethod]
        public void InheritsMessageTest()
        {
            var streamRequestMessage = new StockHistoryRequestMessage();

            Assert.IsNull(streamRequestMessage.MessageID);
            Assert.IsNull(streamRequestMessage.ConversationID);
            Assert.AreEqual(streamRequestMessage.SourceID, 0);
        }

        [TestMethod]
        public void SerializerTest()
        {
            var streamRequestMessage = new StockHistoryRequestMessage
            {
                SourceID = 1,
                ConversationID = "3",
                MessageID = "1"
            };

            var serializedMessage = MessageFactory.GetMessage(streamRequestMessage.Encode(), false) as StockHistoryRequestMessage;

            Assert.AreEqual(streamRequestMessage.TicksRequested, serializedMessage.TicksRequested);
        }
    }
}
