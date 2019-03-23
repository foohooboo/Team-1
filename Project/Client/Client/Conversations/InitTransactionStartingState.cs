using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using System.Net;

namespace Client.Conversations
{
    public class InitTransactionStartingState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InitTransactionStartingState(Conversation conv) : base(conv) {
            
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                case PortfolioUpdateMessage m:
                    Log.Info($"Received PortfolioUpdate message as reply.");
                    //TODO: Update portfolio elements
                    nextState = new ConversationDoneState(ParentConversation, this);
                    break;
                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");
                    nextState = new ConversationDoneState(ParentConversation, this);
                    break;
                default:
                    Log.Error($"No logic to process incoming message of type {incomingMessage.Contents?.GetType()}. Ignoring message.");
                    break;
            }

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return nextState;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            Envelope env = null;

            var m = MessageFactory.GetMessage<TransactionRequestMessage>(
                Config.GetInt(Config.CLIENT_PROCESS_NUM),
                (ParentConversation as InitiateTransactionConversation).PortfoliId
                ) as TransactionRequestMessage;

            env.Contents = m;
            IPAddress.TryParse(Config.GetString(Config.BROKER_IP), out IPAddress ip);
            env.To = new IPEndPoint(ip, Config.GetInt(Config.BROKER_PORT));

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }
    }
}
