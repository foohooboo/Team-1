using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Portfolio;

namespace Broker.Conversations.GetPortfolio
{
    public class GetPortfolioReceiveState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private int PortfolioID
        {
            get; set;
        }

        public GetPortfolioReceiveState(Conversation conversation) : base(conversation, null)
        {

        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {

            if (incomingMessage.Contents is GetPortfolioRequest m)
            {
                PortfolioID = m.Account.PortfolioID;
            }

            return null;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            if (!PortfolioManager.TryToGet(PortfolioID, out Portfolio portfolio))
            {
                // Add handling.
            }

            var message = MessageFactory.GetMessage<PortfolioUpdateMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), PortfolioID) as PortfolioUpdateMessage;

            message.ConversationID = Conversation.Id;
            message.Assets = portfolio.CloneAssets();

            PortfolioManager.ReleaseLock(portfolio);

            var env = new Envelope(message, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT))
            {
                To = this.To
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }
    }
}
