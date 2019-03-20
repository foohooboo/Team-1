using System;
using log4net;
using Shared.Comms.MailService;

namespace Shared.Conversations.SharedStates
{
    public class ConversationDoneState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ConversationState PreviousState;

        public ConversationDoneState(string conversationID, ConversationState previousState) : base(conversationID) {
            PreviousState = previousState;
        }

        public override Envelope Prepare()
        {
            //Conversation should be over, do nothing.
            return null;
        }

        public override ConversationState HandleMessage(Envelope newMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState state = null;
            Log.Warn($"Conversation {ParentConversation.Id} received message while in the Done state. Processing message as if conversation was in the previous state.");
            state = PreviousState?.HandleMessage(newMessage);

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return state;
        }

        public override void HandleTimeout()
        {
            Log.Debug($"{nameof(HandleTimeout)} (enter)");
            
            if (++CountRetrys > Config.GetInt(Config.DEFAULT_RETRY_COUNT))
            {
                Log.Debug($"Conversation {ParentConversation.Id} being cleaned up.");
                PreviousState = null;
                ConversationManager.RemoveConversation(ParentConversation.Id);
            }

            Log.Debug($"{nameof(HandleTimeout)} (exit)");
        }
    }
}
