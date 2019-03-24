using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;

namespace Shared.Conversations.SharedStates
{
    public class InitialState_ConvI_StockStreamRequest : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InitialState_ConvI_StockStreamRequest(Conversation conv) : base(conv) { }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                case StockStreamResponseMessage m:
                    var stockHistory = m.RecentHistory;
                    Log.Info($"Received stock stream response with {stockHistory.Count} days of recent trading.");

                    Temp t = new Temp();
                    t.LogStockHistory(stockHistory);

                    nextState = new ConversationDoneState(ParentConversation, this);
                    break;
                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");
                    nextState = new ConversationDoneState(ParentConversation, this);
                    break;
                default:
                    Log.Error($"No logic to process incoming message of type {incomingMessage.Contents?.GetType()}.");
                    Log.Error($"Ending conversation {ParentConversation.Id}.");
                    nextState = new ConversationDoneState(ParentConversation, this);
                    break;
            }

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return nextState;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            Envelope env = null;

            //Build request message
            var processNum = Config.GetInt(Config.BROKER_PROCESS_NUM);//TODO: allow number to be loaded from broker OR client -dsphar 3/3/2019
            var message = MessageFactory.GetMessage<StockStreamRequestMessage>(processNum, 0);
            message.ConversationID = ParentConversation.Id;
            var stockServerIp = Config.GetString(Config.STOCK_SERVER_IP);
            var stockSerevrPort = Config.GetInt(Config.STOCK_SERVER_PORT);
            env = new Envelope(message, stockServerIp, stockSerevrPort);

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }
    }
}
