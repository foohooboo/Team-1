using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;

namespace Broker.Conversations.GetPortfolio
{
    public class ProcessStockUpdateState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProcessStockUpdateState(Envelope envelope, Conversation conversation) : base(envelope, conversation, null)
        {
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            if (incomingMessage.Contents is StockPriceUpdate m)
            {
                LeaderboardManager.Market.TradedCompanies = m.StocksList.TradedCompanies;
            }

            return null;
        }

        public override Envelope Prepare()
        {
            return null;
        }
    }
}