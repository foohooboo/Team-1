using System;
using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.Portfolio;

namespace Broker.Conversations.TransactionRequest
{
    public class RespondTransaction_InitialState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string InitMessageId;

        public RespondTransaction_InitialState(Conversation conversation, string initMessageId) : base(conversation, null)
        {
            InitMessageId = initMessageId;
        }

        public override ConversationState HandleMessage(Envelope newMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState state = null;

            if (newMessage.Contents.MessageID == InitMessageId)
            {
                //client sent a retry message, resend transaction
                Send();
                state = new ConversationDoneState(Conversation, this);
            }

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return state;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            Envelope env = null;
            bool success = false;

            var processId = Config.GetInt(Config.BROKER_PROCESS_NUM);
            var conv = Conversation as RespondTransactionConversation;


            PortfolioManager.TryToGet(conv.PortfoliId, out Portfolio portfolio);
            Asset change = new Asset(conv.VStock, conv.Quantity);

            //TODO: confirm vStock price is in recent update history
            var totalCost = conv.Quantity * conv.VStock.Close;

            //buying stock
            if (totalCost > 0)
            {
                var dollars = portfolio.GetAsset("$")?.Quantity;
                if (dollars >= totalCost)
                {
                    portfolio.ModifyAsset(change);
                    success = true;
                }
            }
            //selling stock
            else
            {
                var qtyOwned = portfolio.GetAsset(conv.VStock.Symbol)?.Quantity;
                if (qtyOwned > Math.Abs(conv.Quantity))
                {
                    portfolio.ModifyAsset(change);
                    success = true;
                }
            }

            Message message;
            if (success)
            {
                message = MessageFactory.GetMessage<PortfolioUpdateMessage>(
                    processId,
                    portfolio.PortfolioID
                    ) as PortfolioUpdateMessage;
                message.ConversationID = conv.Id;
                (message as PortfolioUpdateMessage).Assets = portfolio.Assets;
            }
            else
            {
                message = MessageFactory.GetMessage<ErrorMessage>(
                    processId,
                    portfolio.PortfolioID
                    ) as ErrorMessage;
                message.ConversationID = conv.Id;
                (message as ErrorMessage).ErrorText = "Broker could not complete transaction.";
            }

            env = new Envelope()
            {
                Contents = message,
                To = conv.ResponseAddress
            };

            PortfolioManager.ReleaseLock(portfolio);

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
