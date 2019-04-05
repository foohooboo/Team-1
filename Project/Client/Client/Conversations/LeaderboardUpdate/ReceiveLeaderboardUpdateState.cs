using System;
using System.Collections;
using log4net;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;

namespace Client.Conversations.LeaderboardUpdate
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

        public ReceiveLeaderboardUpdateState(Envelope env, Conversation conversation) : base(conversation, null)
        {
            Records = (env.Contents as UpdateLeaderBoardMessage).Records;
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
            LeaderboardUpdateEventHandler?.Invoke(this, new LeaderboardUpdateEventArgs(Records));

            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
            return null;
        }
    }
}