using log4net;
using Shared;
using Shared.Client;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Conversations.StockStreamRequest
{
    public class StockStreamRequestState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        long ProcessNumber;

        public StockStreamRequestState(long processNumber, Conversation conversation) : base(conversation, null)
        {
            ProcessNumber = processNumber;
            To = new IPEndPoint(IPAddress.Parse(Config.GetString(Config.STOCK_SERVER_IP)), Config.GetInt(Config.STOCK_SERVER_PORT));
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                case AckMessage m:
                    nextState = new ConversationDoneState(Conversation, this);
                    break;
            }

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return nextState;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            var message = MessageFactory.GetMessage<StockStreamRequestMessage>(ProcessNumber, 0) as StockStreamRequestMessage;
            message.ConversationID = Conversation.Id;
            
            var env = new Envelope(message)
            {
                To = this.To
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }
    }
}
