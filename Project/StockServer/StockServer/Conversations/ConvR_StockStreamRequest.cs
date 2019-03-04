using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using StockServer.Data;

namespace StockServer.Conversations.StockStreamRequest
{
    public class ConvR_StockStreamRequest : Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConvR_StockStreamRequest(Envelope e) : base(e.Contents.ConversationID)
        {
            Log.Debug($"{nameof(ConvR_StockStreamRequest)} (enter)");

            //TODO: save endpoint/connection/postbox for future stock price updates

            var responseMessage = MessageFactory.GetMessage<StockStreamResponseMessage>(Config.GetInt(Config.STOCK_SERVER_PROCESS_NUM), 0) as StockStreamResponseMessage;
            responseMessage.RecentHistory = StockData.GetRecentHistory(5);
            responseMessage.ConversationID = e.Contents?.ConversationID;
            
            var responseEnvelope = new Envelope(responseMessage) { To=e.To};
            var box = PostOffice.GetBox($"0.0.0.0:{Config.GetInt(Config.STOCK_SERVER_PORT)}");
            box.Send(responseEnvelope);

            SetInitialState(new EndConversationState(ConversationId));
            //^Since there is no response to this conversation's first message, we can end the conversation immediately.

            Log.Debug($"{nameof(ConvR_StockStreamRequest)} (exit)");
        }
    }
}
