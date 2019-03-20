using Shared.Comms.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace SharedTest.Messages
{
    [TestClass]
    public class UpdateLeaderBoardMessageTest
    {
        public UpdateLeaderBoardMessageTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var updateLeaderboardMessage = new UpdateLeaderBoardMessage();

            Assert.AreEqual(updateLeaderboardMessage.Records.Count, 0);
        }

        [TestMethod]
        public void InitializerTest()
        {
            var leaderboard = new SortedList
            {
                { 1.1, "foohooboo" },
                { 2.1, "dsphar" }
            };

            var updateLeaderboardMessage = new UpdateLeaderBoardMessage
            {
                Records = leaderboard
            };

            Assert.AreEqual(updateLeaderboardMessage.Records.Count, 2);
            Assert.AreEqual(updateLeaderboardMessage.Records.GetKey(0), 1.1);
            Assert.AreEqual(updateLeaderboardMessage.Records.GetByIndex(0), "foohooboo");

            Assert.AreEqual(updateLeaderboardMessage.Records.GetKey(1), 2.1);
            Assert.AreEqual(updateLeaderboardMessage.Records.GetByIndex(1), "dsphar");
        }

        [TestMethod]
        public void InheritsMessageTest()
        {
            var updateLeaderboardMessage = new UpdateLeaderBoardMessage();

            Assert.AreEqual(updateLeaderboardMessage.SourceID, 0);
            Assert.IsNull(updateLeaderboardMessage.MessageID);
            Assert.IsNull(updateLeaderboardMessage.ConversationID);
        }

        [TestMethod]
        public void SerializerTest()
        {
            var leaderboard = new SortedList
            {
                { 1.1, "foohooboo" },
                { 2.1, "dsphar" }
            };

            var updateLeaderboardMessage = new UpdateLeaderBoardMessage
            {
                Records = leaderboard,
                SourceID = 1,
                ConversationID = "1",
                MessageID = "2"
            };

            var serializedMessage = MessageFactory.GetMessage(updateLeaderboardMessage.Encode(), false) as UpdateLeaderBoardMessage;

            Assert.AreEqual(updateLeaderboardMessage.Records.Count, serializedMessage.Records.Count);
            Assert.AreEqual(updateLeaderboardMessage.Records.GetKey(0).ToString(), serializedMessage.Records.GetKey(0));
            Assert.AreEqual(updateLeaderboardMessage.Records.GetByIndex(0), serializedMessage.Records.GetByIndex(0));

            Assert.AreEqual(updateLeaderboardMessage.Records.GetKey(1).ToString(), serializedMessage.Records.GetKey(1));
            Assert.AreEqual(updateLeaderboardMessage.Records.GetByIndex(1), serializedMessage.Records.GetByIndex(1));
        }
    }
}
