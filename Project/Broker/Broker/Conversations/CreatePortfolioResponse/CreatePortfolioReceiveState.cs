using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Portfolio;

namespace Broker.Conversations.CreatePortfolio
{
    public class CreatePortfolioReceiveState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string Username
        {
            get; set;
        }

        private string Password
        {
            get; set;
        }


        public CreatePortfolioReceiveState(Envelope envelope, Conversation conversation) : base(envelope, conversation, null)
        {
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            if (incomingMessage.Contents is CreatePortfolioRequestMessage m)
            {
                Username = m.Account.Username;
                Password = m.Account.Password;
            }

            return null;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");
            var message = GetMessage();

            var env = new Envelope(message, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT))
            {
                To = this.To
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }

        private Message GetMessage()
        {
            if (!PortfolioManager.TryToCreate(Username, Password, out Portfolio portfolio))
            {
                var errormessage = MessageFactory.GetMessage<ErrorMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as ErrorMessage;

                errormessage.ConversationID = Conversation.Id;
                errormessage.ReferenceMessageID = this.MessageId;

                return errormessage;
            }

            var message = MessageFactory.GetMessage<PortfolioUpdateMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as PortfolioUpdateMessage;

            message.ConversationID = Conversation.Id;
            message.PortfolioID = portfolio.PortfolioID;
            message.Assets = portfolio.CloneAssets();
            PortfolioManager.ReleaseLock(portfolio);

            return message;
        }
    }
}
