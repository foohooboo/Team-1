using Shared.Conversations.SharedStates;

namespace Shared.Conversations.StockStreamRequest.Initiator
{
    class StockStreamRequestConv_Initor : Conversation
    {
        public StockStreamRequestConv_Initor(string conversationId) : base(conversationId)
        {
        }

        public override ConversationState GetInitialState()
        {
            return new InitiateStockStreamRequestState(ConversationId);
        }
    }
}
