using Client.Models;
using log4net;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;
using Shared.PortfolioResources;

namespace Client.Conversations
{
    public class InitTransactionStartingState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ValuatedStock Stock;
        private readonly float Quantity;

        public InitTransactionStartingState(Conversation conv, ValuatedStock vStock, float quantity) : base(conv, null)
        {
            Stock = vStock;
            Quantity = quantity;
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                case PortfolioUpdateMessage m:
                    Log.Info($"Received PortfolioUpdate message as reply.");

                    if (TraderModel.Current != null && TraderModel.Current.Portfolio != null)
                    {
                        var updatedPortfolio = new Portfolio(TraderModel.Current.Portfolio) { Assets = m.Assets };
                        TraderModel.Current.Portfolio = updatedPortfolio;
                    }
                    else
                    {

                        Log.Error("Transaction verified, but no local portfolio to update.");
                    }

                    nextState = new ConversationDoneState(Conversation, this);
                    break;
                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");
                    TraderModel.Current.PassStatus(m.ErrorText);
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
            Log.Debug($"{nameof(Prepare)} (enter)");

            var m = MessageFactory.GetMessage<TransactionRequestMessage>(Config.GetClientProcessNumber(), 0) as TransactionRequestMessage;
            m.ConversationID = Conversation.Id;
            m.Quantity = Quantity;
            m.StockValue = Stock;
            m.PortfolioId = TraderModel.Current?.Portfolio?.PortfolioID ?? -1;

            Envelope env = new Envelope(m, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT));

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }
    }
}
