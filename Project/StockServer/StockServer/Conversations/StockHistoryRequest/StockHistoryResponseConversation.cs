using System.Net;
using log4net;
using Shared.Client;
using Shared.Comms.ComService;
using Shared.Conversations;

namespace StockServer.Conversations.StockStreamRequest
{
    public class StockHistoryResponseConversation : Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly IPEndPoint ClientIp;

        public StockHistoryResponseConversation(Envelope e) : base(e.Contents.ConversationID)
        {
            Log.Debug($"{nameof(StockHistoryResponseConversation)} (enter)");

            ClientIp = e.To;

            Log.Debug($"{nameof(StockHistoryResponseConversation)} (exit)");
        }
    }
}
