using log4net;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using StockServer.Data;

namespace StockServer.Conversations.StockStreamRequest
{
    public class StockHistoryResponseState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string TcpKey;
        private int TicksRequested;

        public StockHistoryResponseState(TcpEnvelope env, Conversation conversation) : base(env, conversation, null)
        {
            TcpKey = env.Key;
            TicksRequested = (env.Contents as StockHistoryRequestMessage).TicksRequested;
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                //no extra cases, as this state is expected to be end of conversation.
                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");
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

            var responseMessage = MessageFactory.GetMessage<StockHistoryResponseMessage>(Config.GetInt(Config.STOCK_SERVER_PROCESS_NUM), 0) as StockHistoryResponseMessage;
            responseMessage.ConversationID = Conversation.Id;
            responseMessage.RecentHistory = StockData.GetRecentHistory(TicksRequested);
                       
            var responseEnvelope = new TcpEnvelope(responseMessage){
                To = this.To,
                Key = TcpKey
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return responseEnvelope;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
            ComService.RemoveClient(To.ToString());
        }
    }
}