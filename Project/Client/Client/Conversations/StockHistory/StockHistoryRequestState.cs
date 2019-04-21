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
                case StockStreamResponseMessage m:
                    Log.Debug($"Received stock history for.");

                    if (TraderModel.Current == null)
                    {
                        Log.Warn("No current TrdaerModel set. Cannot assign incoming stock history");
                    }
                    else
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
            var mes = MessageFactory.GetMessage<StockStreamRequestMessage>(Config.GetClientProcessNumber(), 0);
            mes.ConversationID = Conversation.Id;
            //var env = new Envelope(mes, Config.GetString(Config.STOCK_SERVER_IP), Config.GetInt(Config.STOCK_SERVER_PORT));
            var address = new IPEndPoint(IPAddress.Parse(Config.GetString(Config.STOCK_SERVER_IP)),Config.GetInt(Config.STOCK_SERVER_TCP_PORT));
            var client = ComService.AddTcpClient(0, address);
            var env = new TcpEnvelope(mes)
            {
                To = address,
                Key = client.myTcpClient.Client.RemoteEndPoint.ToString()
            };

            return env;
        }
    }
}
