using System;
using log4net;
using Shared.Comms.MailService;

namespace Shared.Conversations.SharedStates
{
    public class ConversationDoneState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ConversationState PreviousState;

        public ConversationDoneState(string conversationID, ConversationState previousStat) : base(conversationID) {
            PreviousState = previousStat;
        }

        public override void OnStateStart()
        {
            //Conversation should be over, do nothing.
        }

        public override ConversationState GetNextStateFromMessage(Envelope newMessage)
        {
            Log.Debug($"{nameof(GetNextStateFromMessage)} (enter)");

            ConversationState state = null;
            Log.Warn($"Conversation {ConversationID} received message while in the Done state. Processing message as if conversation was in the previous state.");
            state = PreviousState?.GetNextStateFromMessage(newMessage);

            Log.Debug($"{nameof(GetNextStateFromMessage)} (exit)");
            return state;
        }

        public override void HandleTimeout()
        {
            Log.Debug($"{nameof(HandleTimeout)} (enter)");
            
            if (++CountRetrys > Config.GetInt(Config.DEFAULT_RETRY_COUNT))
            {
                Log.Debug($"Conversation {ConversationID} being cleaned up.");
                PreviousState = null;
                ConversationManager.RemoveConversation(ConversationID);
            }

            Log.Debug($"{nameof(HandleTimeout)} (exit)");
        }
    }
}
