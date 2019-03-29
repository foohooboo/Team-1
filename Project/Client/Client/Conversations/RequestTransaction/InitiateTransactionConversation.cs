using Shared;
using Shared.Conversations;
using Shared.MarketStructures;

namespace Client.Conversations
{
    public class InitiateTransactionConversation : Conversation
    {
        public readonly int PortfoliId;
        public readonly ValuatedStock VStock;
        public readonly int Quantity;

        public InitiateTransactionConversation(int portfolioId, ValuatedStock vStock, int quantity) : base(Config.GetInt(Config.CLIENT_PROCESS_NUM))
        {
            PortfoliId = portfolioId;
            VStock = vStock;
            Quantity = quantity;
        }
    }
}
