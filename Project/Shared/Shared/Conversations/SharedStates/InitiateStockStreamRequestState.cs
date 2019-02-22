using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;

namespace Shared.Conversations.SharedStates
{
    class InitiateStockStreamRequestState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InitiateStockStreamRequestState(string conversationID) : base(conversationID)
        {
        }

        public override ConversationState GetNextStateFromMessage(Envelope newMessage)
        {
            if (typeof(StockStreamResponseMessage) == newMessage.GetType())
            {
                //TODO: Handle the response data somehow...
                return new EndConversationState(conversationID);
            }
            //TODO: Add error message handling.
            else
            {
                Log.Error("Could not process incoming message for conversation, ending.");
                return new EndConversationState(conversationID);
            }
        }

        public override void OnStateEnd()
        {
            //Do nothing
        }

        public override void OnStateStart()
        {
            //TODO: Use Post Office to send request message to StockServer
            Log.Info("InitiateStockStreamRequestState OnStateStart running");
        }
    }
}
