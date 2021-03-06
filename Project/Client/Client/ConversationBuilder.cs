﻿using Shared.Comms.ComService;
using Shared.Conversations;
using Shared.Comms.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Conversations.LeaderboardUpdate;
using Client.Conversations.StockUpdate;

namespace Client
{
    public static class ConversationBuilder
    {
        public static Conversation Builder(Envelope env)
        {
            Conversation conv = null;

            switch (env.Contents)
            {
                case UpdateLeaderBoardMessage m:
                    conv = new ReceiveLeaderboardUpdateConversation(m.ConversationID);
                    conv.SetInitialState(new ReceiveLeaderboardUpdateState(env, conv));
                    break;

                case StockPriceUpdate m:
                    conv = new ReceiveStockUpdateConversation(m.ConversationID);
                    conv.SetInitialState(new ReceiveStockUpdateState(env, conv));
                    break;
            }

            return conv;
        }
    }
}
