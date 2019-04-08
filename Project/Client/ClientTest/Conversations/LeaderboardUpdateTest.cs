using Client.Conversations.LeaderboardUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;
using Shared.Security;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClientTest.Conversations
{
    [TestClass]
    public class LeaderboardUpdateTest
    {

        private Mock<ReceiveLeaderboardUpdateState> mock;

        private SignatureService sigServ = new SignatureService();

        public Conversation ConversationBuilder(Envelope env)
        {
            Conversation conv = null;

            switch (env.Contents)
            {
                case UpdateLeaderBoardMessage m:
                    conv = new ReceiveLeaderboardUpdateConversation(m.ConversationID);

                    //setup response message as mock
                    mock = new Mock<ReceiveLeaderboardUpdateState>(env, conv) { CallBase = true };
                    conv.SetInitialState(mock.Object as ReceiveLeaderboardUpdateState);

                    break;
            }

            return conv;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ComService.AddClient(Config.DEFAULT_UDP_CLIENT,0);
            ConversationManager.Start(ConversationBuilder);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ConversationManager.Stop();
            ComService.RemoveClient(Config.DEFAULT_UDP_CLIENT);
        }

        [TestMethod]
        public void Succeed()
        {
            string RequestConvId = "5-562";
            string ClientIp = "192.168.1.31";
            int ClientPort = 5682;

            var testStock = new Stock("TST", "Test Stock");
            ValuatedStock[] vStock = { new ValuatedStock(("1984-02-22,1,2,3,100,5").Split(','), testStock) };
            MarketDay day = new MarketDay("day1", vStock);


            var RequestMessage = new UpdateLeaderBoardMessage()
            {
                ConversationID = RequestConvId,
                SerializedRecords = Convert.ToBase64String(sigServ.Serialize(new SortedList<float, string>()))
            };

            Envelope Request = new Envelope(RequestMessage, ClientIp, ClientPort);

            var localConv = ConversationManager.GetConversation(RequestConvId);
            Assert.IsNull(localConv);
            Assert.IsNull(mock);

            ConversationManager.ProcessIncomingMessage(Request);

            localConv = ConversationManager.GetConversation(RequestConvId);

            Assert.IsNotNull(localConv);
            Assert.IsTrue(localConv.CurrentState is ConversationDoneState);
            mock.Verify(state => state.Prepare(), Times.Once);
            mock.Verify(state => state.Send(), Times.Once);
            mock.Verify(state => state.HandleTimeout(), Times.Never);
        }
    }
} 
