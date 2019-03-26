using System;
using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations.SharedStates;

namespace Shared.Conversations.GetPortfolio
{
    public class GetPortfolioReceiveState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //TODO look at conversation builder in broker

        //TODO update to use the construcor with an envelope
        public GetPortfolioReceiveState(Conversation conversation) : base(conversation)
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

            var message = MessageFactory.GetMessage<PortfolioUpdateMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0);
            
            // populate the message -- add more
            message.ConversationID = ParentConversation.Id;

            // Dismiss portfolio???

            //TODO Get the TO from the received message
            // this will be stored by the constructor after push.

            var env = new Envelope(message, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT));

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }
    }
}
