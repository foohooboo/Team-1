using System.Net;
using log4net;
using Shared.Client;
using Shared.Comms.ComService;
using Shared.Conversations;

namespace StockServer.Conversations.StockStreamRequest
{
    public class ConvR_StockStreamRequest : Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly IPEndPoint ClientIp;

        public ConvR_StockStreamRequest(Envelope e) : base(e.Contents.ConversationID)
        {
            Log.Debug($"{nameof(ConvR_StockStreamRequest)} (enter)");

            ClientIp = e.To;

            ClientManager.TryToAdd(e.To);

            Log.Debug($"{nameof(ConvR_StockStreamRequest)} (exit)");
        }
    }
}
