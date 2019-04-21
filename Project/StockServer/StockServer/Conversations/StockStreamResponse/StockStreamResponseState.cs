using Shared;
using Shared.Client;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServer.Conversations.StockStreamResponse
{
    public class StockStreamResponseState : ConversationState
    {
        public StockStreamResponseState(Envelope env, Conversation conversation) : base(env, conversation, null)
        {
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            return null;
        }

        public override Envelope Prepare()
        {
            ClientManager.TryToAdd(To);

            var mes = MessageFactory.GetMessage<AckMessage>(Config.GetInt(Config.STOCK_SERVER_PROCESS_NUM), 0) as AckMessage;
            mes.ConversationID = Conversation.Id;

            var env = new Envelope(mes)
            {
                To = this.To
            };
            return env;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
        }
    }
}
