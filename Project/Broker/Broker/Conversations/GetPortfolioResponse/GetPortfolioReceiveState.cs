using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;

namespace Broker.Conversations.GetPortfolio
{
    public class GetPortfolioReceiveState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly int portfolioID;
        //TODO look at conversation builder in broker

        //TODO update to use the construcor with an envelope
        public GetPortfolioReceiveState(Conversation conversation) : base(conversation, null)
        {
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            return null;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            // Get the portfolio from the portfolio manager.
            //if (!PortfolioManager.TryToGet()
            //if(!PortfolioManager.)

            var message = MessageFactory.GetMessage<PortfolioUpdateMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0);

            // Deep copy of portfolio

            // populate the message -- add more
            message.ConversationID = Conversation.Id;

            // Dismiss portfolio?

            //TODO Get the TO from the received message
            // this will be stored by the constructor after push.

            var env = new Envelope(message, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT));

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }
    }
}
