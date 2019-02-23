using Shared.Comms.MailService;

namespace Shared.Conversations
{
    public abstract class ConversationState
    {
        public readonly string ConversationID;

        public ConversationState(string conversationID)
        {
            ConversationID = conversationID;
        }

        public abstract ConversationState GetNextStateFromMessage(Envelope newMessage);
        public abstract void OnStateStart();
        public abstract void OnStateEnd();
    }
}
