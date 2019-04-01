using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using StockServer.Data;

namespace StockServer.Conversations.StockUpdate
{
    public class StockUpdateSendState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Use this constructor if state is created locally and not from incoming message.
        public StockUpdateSendState(Conversation conv, ConversationState previousState) : base(conv, previousState) { }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            return null;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            var message = MessageFactory.GetMessage<StockPriceUpdate>(Config.GetInt(Config.STOCK_SERVER_PROCESS_NUM), 0) as StockPriceUpdate;

            message.StocksList = StockData.GetCurrentDay();

            var env = new Envelope(message)
            {
                To = this.To
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }
    }
}
