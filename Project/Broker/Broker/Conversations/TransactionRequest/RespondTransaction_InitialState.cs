using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.Portfolio;
using System;

namespace Broker.Conversations.TransactionRequest
{
    public class RespondTransaction_InitialState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RespondTransaction_InitialState(Conversation conversation) : base(conversation)
        {
        }

        public override ConversationState HandleMessage(Envelope newMessage)
        {
            throw new NotImplementedException();
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            Envelope env = null;
            bool success = false;

            var processId = Config.GetInt(Config.BROKER_PROCESS_NUM);
            var conv = ParentConversation as RespondTransactionConversation;
            var portfolio = PortfolioManager.GetPortfolio(conv.PortfoliId);
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
                if(qtyOwned > Math.Abs(conv.Quantity))
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
                (message as ErrorMessage).ErrorText = "Could not complete transaction.";
            }

            env = new Envelope()
            {
                Contents = message,
                To = conv.ResponseAddress
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }

        public override void Send()
        {
            base.Send();
            ParentConversation.UpdateState(new ConversationDoneState(ParentConversation, this));
        }
    }
}
