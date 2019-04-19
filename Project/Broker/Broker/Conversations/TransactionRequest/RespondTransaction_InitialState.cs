using log4net;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.PortfolioResources;

namespace Broker.Conversations.TransactionRequest
{
    public class RespondTransaction_InitialState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        TransactionRequestMessage Request;

        public RespondTransaction_InitialState(Conversation conversation, Envelope env) : base(env, conversation, null)
        {
            Request = env.Contents as TransactionRequestMessage;
        }

        public override ConversationState HandleMessage(Envelope newMessage)
        {
            Log.Warn($"Respond Transaction initial state did not expect to handle a message.");
            return this;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            var success = PortfolioManager.PerformTransaction(
                Request.PortfolioId,
                Request.StockValue.Symbol,
                Request.Quantity,
                Request.StockValue.Close,
                out Portfolio updatedPortfolio,
                out string errorMessage);

            Envelope env;

            if (success)
            {
                var reply = MessageFactory.GetMessage<PortfolioUpdateMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as PortfolioUpdateMessage;
                reply.ConversationID = Conversation.Id;
                reply.Assets = updatedPortfolio.Assets;
                reply.PortfolioID = updatedPortfolio.PortfolioID;
                env = new Envelope(reply) { To = this.To };
            }
            else
            {
                var reply = MessageFactory.GetMessage<ErrorMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as ErrorMessage;
                reply.ConversationID = Conversation.Id;
                reply.ErrorText = errorMessage;
                env = new Envelope(reply) { To = this.To };
            }

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
        }
    }
}
