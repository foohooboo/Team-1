using Shared.Conversations;
using Shared.Conversations.SharedStates;

namespace Broker.Conversations.StockHistoryRequest
{
    public class StockHistoryRequestConversation : Conversation
    {
        public StockHistoryRequestConversation(int processId):base(processId)
        {
        }
    }
}
