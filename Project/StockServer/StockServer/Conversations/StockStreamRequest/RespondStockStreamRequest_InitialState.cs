using log4net;
using Shared;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using StockServer.Data;

namespace StockServer.Conversations.StockStreamRequest
{
    public class RespondStockStreamRequest_InitialState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RespondStockStreamRequest_InitialState(Envelope env, Conversation conversation) : base(env, conversation, null)
        {

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

            var conv = Conversation as ConvR_StockStreamRequest;

            var responseMessage = MessageFactory.GetMessage<StockStreamResponseMessage>(Config.GetInt(Config.STOCK_SERVER_PROCESS_NUM), 0) as StockStreamResponseMessage;
            responseMessage.ConversationID = conv.Id;
            responseMessage.RecentHistory = StockData.GetRecentHistory(5);

            new Temp().LogStockHistory(responseMessage.RecentHistory);//log to console for prelim dev. Remove once not needed.

            var responseEnvelope = new Envelope(responseMessage) { To = conv.ClientIp };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return responseEnvelope;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
        }
    }
}