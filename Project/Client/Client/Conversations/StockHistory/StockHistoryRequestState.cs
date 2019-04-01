using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                case PortfolioUpdateMessage m:
                    Log.Debug($"Received portfolio for ...\n{m.PortfolioID}");

                    // TODO: Update portfolio model data.

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
            var mes = MessageFactory.GetMessage<StockStreamRequestMessage>(Config.GetInt(Config.CLIENT_PROCESS_NUM),0);
            mes.ConversationID = Conversation.Id;
            var env = new Envelope(mes, Config.GetString(Config.STOCK_SERVER_IP), Config.GetInt(Config.STOCK_SERVER_PORT));
            return env;
        }
    }
}
