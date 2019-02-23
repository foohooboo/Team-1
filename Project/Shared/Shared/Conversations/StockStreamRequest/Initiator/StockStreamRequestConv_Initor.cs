using Shared.Conversations.SharedStates;

namespace Shared.Conversations.StockStreamRequest.Initiator
{
    class StockStreamRequestConv_Initor : InitiatorConversation
    {
        public StockStreamRequestConv_Initor(int processId, ConversationState initialState):base(initialState)
        {
        }
    }
}
