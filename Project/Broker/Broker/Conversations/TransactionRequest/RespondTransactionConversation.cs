using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.MarketStructures;
using System.Net;

namespace Broker.Conversations.TransactionRequest
{
    public class RespondTransactionConversation : Conversation
    {
        public readonly int PortfoliId;
        public readonly ValuatedStock VStock;
        public readonly int Quantity;
        public readonly IPEndPoint ResponseAddress;

        public RespondTransactionConversation(TransactionRequestMessage reqMessage, IPEndPoint responseAddress) :  base(reqMessage.ConversationID)
        {
            PortfoliId = reqMessage.PortfolioId;
            VStock = reqMessage.StockValue;
            Quantity = reqMessage.Quantity;
            ResponseAddress = responseAddress;
        }
    }
}
