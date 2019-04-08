﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using log4net;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.Security;

namespace Client.Conversations.LeaderboardUpdate
{
    public class ReceiveLeaderboardUpdateState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SortedList<float,string> Records
        {
            get; private set;
        }

        public ReceiveLeaderboardUpdateState(Envelope env, Conversation conversation) : base(env, conversation, null)
        {
            var sigServe = new SignatureService();
            Records = sigServe.Deserialize<SortedList<float,string>>(Convert.FromBase64String((env.Contents as UpdateLeaderBoardMessage).SerializedRecords));
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            //This state doesn't expect any incoming messages

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return null;
        }

        public override Envelope Prepare()
        {
            //Update records
            if (Records == null)
            {
                Log.Error("Leaderboard update message was missing the leaderboard.");
            }
            else
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    TraderModel.Current.Leaderboard = this.Records;
                });
            }
            
            var ack = MessageFactory.GetMessage<AckMessage>(Config.GetInt(Config.CLIENT_PROCESS_NUM),0);
            ack.ConversationID = Conversation?.Id;

            var env = new Envelope(ack)
            {
                To = this.To
            };

            return env;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
        }
    }
}