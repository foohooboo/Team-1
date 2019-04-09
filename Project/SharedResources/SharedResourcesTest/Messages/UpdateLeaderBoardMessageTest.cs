using Shared.Comms.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using Shared.Security;
using System;
using Shared.MarketStructures;
using System.Collections.Generic;
using Shared.Leaderboard;

namespace SharedTest.Messages
{
    [TestClass]
    public class UpdateLeaderBoardMessageTest
    {

        private SignatureService sigServ = new SignatureService();

        public UpdateLeaderBoardMessageTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var updateLeaderboardMessage = new UpdateLeaderBoardMessage();

            Assert.AreEqual(updateLeaderboardMessage.SerializedRecords, "");
        }

        [TestMethod]
        public void InitializerTest()
        {
            var leaderboard = new SortedList<float, string>();
            leaderboard.Add(1.1f, "foohooboo");
            leaderboard.Add(2.1f, "dsphar");

            var updateLeaderboardMessage = new UpdateLeaderBoardMessage
            {
                SerializedRecords = Convert.ToBase64String(sigServ.Serialize(leaderboard))
            };

            var records = sigServ.Deserialize<SortedList<float, string>>(Convert.FromBase64String(updateLeaderboardMessage.SerializedRecords));

            Assert.AreEqual(records.Count, 2);
            Assert.AreEqual(records.Keys[0], 1.1f);
            Assert.AreEqual(records.Values[0].ToString(), "foohooboo");

            Assert.AreEqual(records.Keys[1], 2.1f);
            Assert.AreEqual(records.Values[1].ToString(), "dsphar");
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
            var leaderboard = new SortedList<float, string>();
            leaderboard.Add(1.1f, "foohooboo");
            leaderboard.Add(2.1f, "dsphar");

            var updateLeaderboardMessage = new UpdateLeaderBoardMessage
            {
                SerializedRecords = Convert.ToBase64String(sigServ.Serialize(leaderboard)),
                SourceID = 1,
                ConversationID = "1",
                MessageID = "2"
            };

            var serializedMessage = MessageFactory.GetMessage(updateLeaderboardMessage.Encode(), false) as UpdateLeaderBoardMessage;

            var records = sigServ.Deserialize<SortedList<float, string>>(Convert.FromBase64String(updateLeaderboardMessage.SerializedRecords));
            var records_fromSerialized = sigServ.Deserialize<SortedList<float, string>>(Convert.FromBase64String(serializedMessage.SerializedRecords));

            Assert.AreEqual(records.Count, records_fromSerialized.Count);

            Assert.AreEqual(records.Keys[0], records_fromSerialized.Keys[0]);
            Assert.AreEqual(records.Values[0], records_fromSerialized.Values[0]);

            Assert.AreEqual(records.Keys[1], records_fromSerialized.Keys[1]);
            Assert.AreEqual(records.Values[1], records_fromSerialized.Values[1]);
        }
    }
}
