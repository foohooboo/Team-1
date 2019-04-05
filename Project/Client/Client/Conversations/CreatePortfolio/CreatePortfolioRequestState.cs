using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using System;
using Shared.PortfolioResources;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Shared.Conversations.SharedStates;

namespace Client.Conversations.CreatePortfolio
{
    public class CreatePortfolioRequestState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string Username;
        private string Password;
        private string ConfirmPassword;

        public CreatePortfolioRequestState(string username, string password, string confirmPassword, Conversation conversation, ConversationState previousState) : base(conversation, previousState)
        {
            Username = username;
            Password = password;
            ConfirmPassword = confirmPassword;
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
                    nextState = new ConversationDoneState(Conversation, this);
                    break;
                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");
                    nextState = new ConversationDoneState(Conversation, this);
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
            var mes = MessageFactory.GetMessage<CreatePortfolioRequestMessage>(Config.GetInt(Config.CLIENT_PROCESS_NUM), 0) as CreatePortfolioRequestMessage;
            mes.Account = new Portfolio()
            {
                Username = this.Username,
                Password = this.Password
            };
            mes.ConfirmPassword = ConfirmPassword;
            mes.ConversationID = Conversation.Id;

            return new Envelope(mes, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT));
        }
    }
}
