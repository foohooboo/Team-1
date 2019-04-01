using System;
using System.Collections;
using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;

namespace Client.Conversations.StockUpdate
{
    public class ReceiveLeaderboardUpdateState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SortedList Records
        {
            get; private set;
        }

        public static event EventHandler<LeaderboardUpdateEventArgs> LeaderboardUpdateEventHandler;

        public class LeaderboardUpdateEventArgs : EventArgs
        {
            public SortedList Records
            {
                get; private set;
            }

            public LeaderboardUpdateEventArgs(SortedList records)
            {
                Records = new SortedList(records);
            }
        }

        public ReceiveLeaderboardUpdateState(Envelope env, Conversation conversation, ConversationState parentState) : base(conversation, parentState)
        {

        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState state = null;

            if (incomingMessage.Contents is UpdateLeaderBoardMessage m)
            {
                state = new ConversationDoneState(Conversation, this);
                Records = m.Records;
            }

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return state;
        }

        public override Envelope Prepare()
        {
            LeaderboardUpdateEventHandler?.Invoke(this, new LeaderboardUpdateEventArgs(Records));
            return null;
        }
    }
}