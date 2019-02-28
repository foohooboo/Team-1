using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations.SharedStates;

namespace Shared.Conversations.StockStreamRequest
{
    class ConvR_StockStreamRequest : Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConvR_StockStreamRequest(Envelope e) : base(e.Contents.ConversationID)
        {
            Log.Debug($"{nameof(ConvR_StockStreamRequest)} (enter)");

            //TODO: save endpoint/connection/postbox for future stock price updates
            //Note: Daniel is working on how this endpoint/connection will be persisted from the communicator standpoint.

            var responseMessage = MessageFactory.GetMessage<StockStreamResponseMessage>(1, 2);//TODO: remove process and portfolio id magic number hacks
            
            //TODO: Add stock data history to responseMessage
            
            var responseEnvelope = new Envelope(responseMessage);
            responseEnvelope.To = e.From;
            //TODO: Change envelope to UDP or TCP. Send to communicator for transmission.
            //This is likely to change once Daniel works out persistence in post boxes. Either way, .
            
            SetInitialState(new EndConversationState(ConversationId));
            //^Since there is no response to this conversation's first message, we can end the conversation immediately.

            Log.Debug($"{nameof(ConvR_StockStreamRequest)} (exit)");
        }
    }
}
