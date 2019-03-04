using Shared.Conversations.SharedStates;

namespace Shared.Conversations.StockStreamRequest.Initiator
{
    class ConvI_StockStreamRequest : InitiatorConversation
    {
        public string StockServerAddress { get; set; }

        public ConvI_StockStreamRequest(ConversationState initialState):base(initialState)
        {
        }
    }
}
