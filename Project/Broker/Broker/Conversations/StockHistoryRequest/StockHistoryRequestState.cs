using log4net;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared;
using System.Net;
using SharedResources.Conversations.StockStreamRequest;

namespace Broker.Conversations.StockHistoryRequest
{
    public class StockHistoryRequestState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StockHistoryRequestState(Conversation conv) : base(conv, null) { }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                case StockHistoryResponseMessage m:
                    var stockHistory = m.RecentHistory;
                    Log.Info($"Received stock stream response with {stockHistory.Count} days of recent trading.");
                    nextState = new ConversationDoneState(Conversation, this);

                    var streamConv = new StockStreamRequestConversation(Config.GetInt(Config.BROKER_PROCESS_NUM));
                    streamConv.SetInitialState(new StockStreamRequestState(Config.GetInt(Config.BROKER_PROCESS_NUM), streamConv));
                    ConversationManager.AddConversation(streamConv);

                    break;
                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");
                    nextState = new ConversationDoneState(Conversation, this);
                    break;
                default:
                    Log.Error($"No logic to process incoming message of type {incomingMessage.Contents?.GetType()}.");
                    Log.Error($"Ending conversation {Conversation.Id}.");
                    nextState = new ConversationDoneState(Conversation, this);
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
            var processNum = Config.GetInt(Config.BROKER_PROCESS_NUM);
            var message = MessageFactory.GetMessage<StockHistoryRequestMessage>(processNum, 0);
            message.ConversationID = Conversation.Id;
            var stockServerIp = Config.GetString(Config.STOCK_SERVER_IP);
            var stockSerevrPort = Config.GetInt(Config.STOCK_SERVER_TCP_PORT);

            var address = new IPEndPoint(IPAddress.Parse(stockServerIp), stockSerevrPort);
            ComService.AddTcpClient(0, address);

            env = new TcpEnvelope(message, Config.GetString(Config.STOCK_SERVER_IP), Config.GetInt(Config.STOCK_SERVER_TCP_PORT));

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }
    }
}
