﻿using System;
using System.Threading;
using Client.Conversations;
using Client.Conversations.StockUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;

namespace ClientTest.Conversations
{
    [TestClass]
    public class StockUpdateTest
    {

        private Mock<ReceiveStockUpdateState> mock;

        public Conversation ConversationBuilder(Envelope env)
        {
            Conversation conv = null;

            switch (env.Contents)
            {
                case StockPriceUpdate m:
                    conv = new ReceiveStockUpdateConversation(m.ConversationID);

                    //setup response message as mock
                    mock = new Mock<ReceiveStockUpdateState>(env, conv) { CallBase = true };
                    conv.SetInitialState(mock.Object as ReceiveStockUpdateState);

                    break;
            }

            return conv;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ComService.AddUdpClient(Config.DEFAULT_UDP_CLIENT,0);
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


            var RequestMessage = new StockPriceUpdate(day)
            {
                ConversationID = RequestConvId,
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
