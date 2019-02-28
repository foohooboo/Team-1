using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Conversations;

namespace Broker
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
            throw new System.NotImplementedException();
        }

        public override void OnStateEnd()
        {
            throw new System.NotImplementedException();
        }

        public override void OnStateStart()
        {
            Log.Debug($"{nameof(OnStateStart)} (enter)");

            Log.Debug($"{nameof(OnStateStart)} (exit)");
        }
    }
}