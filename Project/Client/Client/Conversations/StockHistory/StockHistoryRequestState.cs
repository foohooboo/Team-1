using Client.Models;
using log4net;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using System.Net;

namespace Client.Conversations.StockHistory
{
    public class StockHistoryRequestState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StockHistoryRequestState(Conversation conversation) : base(conversation, null)
        {

        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                case StockHistoryResponseMessage m:
                    Log.Debug($"Received stock history for.");

                    if (TraderModel.Current != null)
                    {
                        TraderModel.Current.StockHistory = m.RecentHistory;
                    }
                    
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

            return nextState;
        }

        public override Envelope Prepare()
        {
            var mes = MessageFactory.GetMessage<StockHistoryRequestMessage>(Config.GetClientProcessNumber(), 0);
            mes.ConversationID = Conversation.Id;

            var stockServerIp = Config.GetString(Config.STOCK_SERVER_IP);
            var stockSerevrPort = Config.GetInt(Config.STOCK_SERVER_TCP_PORT);
            var stockServer = new IPEndPoint(IPAddress.Parse(stockServerIp), stockSerevrPort);
            ComService.AddTcpClient(0, stockServer);

            var env = new TcpEnvelope(mes, stockServerIp, stockSerevrPort);

            return env;
        }
    }
}
