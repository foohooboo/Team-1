using Shared.Comms.Messages;

namespace Shared.Conversations.States
{
    public abstract class ConversationState
    {
        public ConversationState(Conversation owner)
        {
            conversation = owner;
        }

        public abstract ConversationState ProcessMessage(Message newMessage);

        private Conversation conversation;
    }
}
