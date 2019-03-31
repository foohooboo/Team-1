using log4net;
using Shared;
using Shared.Client;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.PortfolioResources;

namespace Broker.Conversations.GetPortfolio
{
    public class GetPortfolioReceiveState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int PortfolioID
        {
            get; set;
        }

        public GetPortfolioReceiveState(Envelope envelope, Conversation conversation) : base(envelope, conversation, null)
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
            var message = GetMessage();

            // If we retreived a portfolio, add the receiver to the client list.
            if (message is PortfolioUpdateMessage m)
            {
                ClientManager.TryToAdd(To);
            }

            var env = new Envelope(message, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT))
            {
                To = this.To
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }

        private Message GetMessage()
        {
            if (!PortfolioManager.TryToGet(PortfolioID, out Portfolio portfolio))
            {
                var errormessage = MessageFactory.GetMessage<ErrorMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), PortfolioID) as ErrorMessage;

                errormessage.ConversationID = Conversation.Id;
                errormessage.ReferenceMessageID = this.MessageId;

                return errormessage;
            }

            var message = MessageFactory.GetMessage<PortfolioUpdateMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), PortfolioID) as PortfolioUpdateMessage;

            message.ConversationID = Conversation.Id;
            message.Assets = portfolio.CloneAssets();
            message.PortfolioID = portfolio.PortfolioID;
            PortfolioManager.ReleaseLock(portfolio);

            return message;
        }
    }
}
