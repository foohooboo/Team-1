using Shared;
using Shared.Conversations;
using Shared.MarketStructures;

namespace Client.Conversations
{
    public class InitiateTransactionConversation : Conversation
    {
        public readonly int PortfoliId;

        public InitiateTransactionConversation(int portfolioId) : base(Config.GetClientProcessNumber())
        {
            PortfoliId = portfolioId;
        }
    }
}
