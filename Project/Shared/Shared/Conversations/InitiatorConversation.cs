namespace Shared.Conversations
{
    public abstract class InitiatorConversation : Conversation
    {
        public InitiatorConversation(ConversationState startingState) : base(startingState.ConversationID)
        {
            SetInitialState(startingState);
        }
    }
}
