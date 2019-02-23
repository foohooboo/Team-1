using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations.SharedStates;
using System.Net;

namespace Shared.Conversations.StockStreamRequest
{
    class ConvR_StockStreamRequest : Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConvR_StockStreamRequest(Envelope e) : base(e.Contents.ConversationID)
        {
            Log.Debug($"Enter - {nameof(ConvR_StockStreamRequest)}");

            var responseMessage = MessageFactory.GetMessage<StockStreamResponseMessage>(1, 2);//TODO: remove process and portfolio id magic number hacks
            var responseEnvelope = new Envelope(responseMessage);
            responseEnvelope.To = e.From;
            //TODO: Change envelope to UDP or TCP. Send to communicator for transmission.
            
            SetInitialState(new EndConversationState(ConversationId));
            //^Since there is no response to this conversation's first message, we can end the conversation immediately.

            Log.Debug($"Enter - {nameof(ConvR_StockStreamRequest)}");
        }
    }
}
