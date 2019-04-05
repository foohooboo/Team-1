using log4net;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;

namespace Broker.Conversations
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
            //TODO: We need to trigger the Leaderboard update request conversation at this point some how.

            var ack = MessageFactory.GetMessage<AckMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM),0);
            ack.ConversationID = Conversation.Id;
            var env = new Envelope(ack)
            {
                To = this.To
            };

            return env;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
        }
    }
}