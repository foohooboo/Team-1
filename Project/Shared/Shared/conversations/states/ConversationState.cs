using Shared.comms.messages;

namespace Shared.conversations.states
{
    public abstract class ConversationState
    {
        public ConversationState(Conversation owner)
        {
            conversation = owner;
        }

        public abstract ConversationState ProcessMessage(Message newMessage);
        public void EndConversation()
        {
            conversation.End();
        }

        private Conversation conversation;
    }
}
