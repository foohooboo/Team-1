using System;
using log4net;
using Shared.Comms.ComService;

namespace Shared.Conversations.SharedStates
{
    public class ConversationDoneState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConversationDoneState(Conversation conv, ConversationState previousState) : base(conv, previousState) {
            
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
            Log.Warn($"Conversation {Conversation.Id} received message while in the Done state. Processing message as if conversation was in the previous state.");
            state = PreviousState?.OnHandleMessage(newMessage, 0);

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return state;
        }

        public override void HandleTimeout()
        {
            Log.Debug($"{nameof(HandleTimeout)} (enter)");
            
            if (++CountRetrys >= Config.GetInt(Config.DEFAULT_RETRY_COUNT))
            {
                Log.Debug($"Conversation {Conversation.Id} being cleaned up.");

                ConversationManager.RemoveConversation(Conversation.Id);
            }

            Log.Debug($"{nameof(HandleTimeout)} (exit)");
        }
    }
}
