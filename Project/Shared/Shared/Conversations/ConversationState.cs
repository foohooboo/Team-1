using Shared.Comms.MailService;

namespace Shared.Conversations
{
    public abstract class ConversationState
    {
        protected readonly string conversationID;

        public ConversationState(string conversationID)
        {
            this.conversationID = conversationID;
        }

        public abstract ConversationState GetNextStateFromMessage(Envelope newMessage);
        public abstract void OnStateStart();
        public abstract void OnStateEnd();
    }
}
