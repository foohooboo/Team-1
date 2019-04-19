using log4net;
using Shared;
using Shared.Client;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.PortfolioResources;
using System.Linq;

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

        private string ConfirmPassword
        {
            get; set;
        }


        public CreatePortfolioReceiveState(Envelope envelope, Conversation conversation) : base(envelope, conversation, null)
        {
            if (!(envelope.Contents is CreatePortfolioRequestMessage request))
                throw new System.Exception("CreatePortflioReceieveState requires CreatePortfolioRequestMessage.");

            Username = request.Account.Username;
            Password = request.Account.Password;
            ConfirmPassword = request.ConfirmPassword;
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

            // If we created a portfolio, add the receiver to the client list.
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
            var errorMessage = "";
            Portfolio portfolio = null;

            if (!Password.Equals(ConfirmPassword))
                errorMessage = "Passwords do not match";
            else if (PortfolioManager.Portfolios.Any(p=>p.Value.Username.Equals(Username)))
                errorMessage = "A portfolio with that name already exists.";
            else if (!PortfolioManager.TryToCreate(Username, Password, out portfolio))
                errorMessage = "Broker could not create portfolio.";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                var errormessage = MessageFactory.GetMessage<ErrorMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as ErrorMessage;
                errormessage.ErrorText = errorMessage;
                errormessage.ConversationID = Conversation.Id;
                errormessage.ReferenceMessageID = this.MessageId;
                return errormessage;
            }

            var message = MessageFactory.GetMessage<PortfolioUpdateMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as PortfolioUpdateMessage;

            message.ConversationID = Conversation.Id;
            message.PortfolioID = portfolio.PortfolioID;
            message.Assets = portfolio.CloneAssets();

            return message;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
        }
    }
}