using log4net;
using Shared;
using Shared.Client;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.PortfolioResources;
using System;

namespace Broker.Conversations.GetPortfolio
{
    public class GetPortfolioReceiveState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string PortfolioName
        {
            get; set;
        }

        private string PortfolioPassword
        {
            get; set;
        }

        public GetPortfolioReceiveState(Envelope envelope, Conversation conversation) : base(envelope, conversation, null)
        {
            Log.Debug($"{nameof(GetPortfolioReceiveState)} (enter)");

            if (!(envelope.Contents is GetPortfolioRequest m))
                throw new ArgumentException("GetPortfolioReceiveState must be created with a GetPortfolioRequest message.");

            PortfolioName = m.Account.Username;
            PortfolioPassword = m.Account.Password;
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            //This state should never handle a new message
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
            if (!PortfolioManager.TryToGet(PortfolioName, PortfolioPassword, out Portfolio portfolio))
            {
                var errormessage = MessageFactory.GetMessage<ErrorMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as ErrorMessage;

                errormessage.ConversationID = Conversation.Id;
                errormessage.ReferenceMessageID = this.MessageId;

                return errormessage;
            }

            var message = MessageFactory.GetMessage<PortfolioUpdateMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), portfolio.PortfolioID) as PortfolioUpdateMessage;

            message.ConversationID = Conversation.Id;
            message.Assets = portfolio.CloneAssets();
            message.PortfolioID = portfolio.PortfolioID;
            PortfolioManager.ReleaseLock(portfolio);

            return message;
        }

        public override void Send()
        {
            Log.Debug($"{nameof(Send)} (enter)");
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
            Log.Debug($"{nameof(Send)} (exit)");
        }
    }
}
