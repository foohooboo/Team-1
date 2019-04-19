using Shared.Comms.ComService;
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
        public readonly float Quantity;
        public readonly IPEndPoint ResponseAddress;

        public RespondTransactionConversation(Envelope e) :  base(e.Contents.ConversationID)
        {
            var reqMessage = e.Contents as TransactionRequestMessage;
            PortfoliId = reqMessage.PortfolioId;
            VStock = reqMessage.StockValue;
            Quantity = reqMessage.Quantity;
            ResponseAddress = e.To;
        }
    }
}
