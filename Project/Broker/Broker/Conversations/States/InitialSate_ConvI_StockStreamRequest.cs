using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using System.Net;

namespace Broker.Conversations.States
{
    class InitialSate_ConvI_StockStreamRequest : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InitialSate_ConvI_StockStreamRequest() : 
            base(ConversationManager.GenerateNextId(Config.GetInt(Config.BROKER_PROCESS_NUM)))
        {
            
        }

        public override ConversationState GetNextStateFromMessage(Envelope newMessage)
        {
            Log.Debug($"{nameof(GetNextStateFromMessage)} (enter)");

            throw new System.NotImplementedException();

            Log.Debug($"{nameof(GetNextStateFromMessage)} (exit)");
        }

        public override void OnStateEnd()
        {
            
        }

        public override void OnStateStart()
        {
            Log.Debug($"{nameof(OnStateStart)} (enter)");

            var message = MessageFactory.GetMessage<StockStreamRequestMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0);
            var env = new Envelope(message);
            //TODOL Set env destination to StockStreamServer listener port as defined in Config
            //TODO: Send env

            Log.Debug($"{nameof(OnStateStart)} (exit)");
        }
    }
}