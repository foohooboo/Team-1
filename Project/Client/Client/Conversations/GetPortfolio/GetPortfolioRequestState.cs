using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.PortfolioResources;
using System.Threading.Tasks;

namespace Client.Conversations.GetPortfolio
{
    public class GetPortfolioRequestState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string Username;
        private string Password;
        private IHandleLogin LoginHandler;

        public GetPortfolioRequestState(string username, string password, IHandleLogin loginHandler, Conversation conversation) : base(conversation, null)
        {
            Username = username;
            Password = password;
            LoginHandler = loginHandler;
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {

                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");

                    Task.Run(() => LoginHandler?.LoginFailure(m.ErrorText)); 
                    
                    nextState = new ConversationDoneState(Conversation, this);
                    ConversationManager.RemoveConversation(Conversation.Id);
                    break;

                case PortfolioUpdateMessage m:
                    Log.Debug($"Received portfolio for ...\n{m.PortfolioID}");

                    var port = new Portfolio()
                    {
                        Assets = m.Assets,
                        Username = Username,
                        PortfolioID = m.PortfolioID
                    };

                    // TODO: Update portfolio model data.

                    nextState = new ConversationDoneState(Conversation, this);
                    ConversationManager.RemoveConversation(Conversation.Id);

                    Task.Run(() => LoginHandler?.LoginSuccess(port));
                    
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

            var message = MessageFactory.GetMessage<GetPortfolioRequest>(Config.GetInt(Config.CLIENT_PROCESS_NUM), 0) as GetPortfolioRequest;
            message.ConversationID = Conversation.Id;
            message.Account = new Portfolio()
            {
                Username = this.Username,
                Password = this.Password
            };

            var env = new Envelope(message, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT));

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }

        public override void HandleTimeout()
        {
            if (++CountRetrys <= Config.GetInt(Config.DEFAULT_RETRY_COUNT))
            {
                Log.Warn($"Initiating retry for conversation {Conversation.Id}.");
                Send();
            }
            else
            {
                Log.Warn($"Timeout event is forcing conversation {Conversation.Id} into the Done state.");
                Conversation.UpdateState(new ConversationDoneState(Conversation, this));
                LoginHandler?.LoginFailure("Login attempt timed out. Is the Broker running?");
            }
        }
    }
}
