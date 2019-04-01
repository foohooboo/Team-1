using System.Net;
using log4net;
using Shared;
using Shared.Client;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;
using StockServer.Data;

namespace StockServer.Conversations.StockUpdate
{
    public class StockUpdateSendState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private MarketDay DayData;

        public StockUpdateSendState(MarketDay dayData, IPEndPoint recipiant, Conversation conv, ConversationState previousState) : base(conv, previousState)
        {
            To = recipiant;
            DayData = dayData;
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                case AckMessage m:
                    nextState = new ConversationDoneState(Conversation, this);
                    break;
            }

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return nextState;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            var message = MessageFactory.GetMessage<StockPriceUpdate>(Config.GetInt(Config.STOCK_SERVER_PROCESS_NUM), 0) as StockPriceUpdate;

            message.StocksList = DayData;

            var env = new Envelope(message)
            {
                To = this.To
            };

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
                ConversationManager.GetConversation(Conversation.Id).UpdateState(new ConversationDoneState(Conversation, this));
                Log.Info($"Client {OutboundMessage.To.ToString()} appears to be disconnected. Removing from connected clients.");
                ClientManager.TryToRemove(OutboundMessage.To);
            }
        }
    }
}