using Shared;
using Shared.Conversations;

namespace Client.Conversations.StockHistory
{
    public class StockHistoryRequestConversation : Conversation
    {
        public StockHistoryRequestConversation() : base(Config.GetClientProcessNumber())
        {

        }
    }
}
