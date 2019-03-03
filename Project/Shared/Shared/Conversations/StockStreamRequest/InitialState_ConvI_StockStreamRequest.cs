using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;

namespace Shared.Conversations.SharedStates
{
    class InitialState_ConvI_StockStreamRequest : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InitialState_ConvI_StockStreamRequest(int processNum) : base(ConversationManager.GenerateNextId(processNum)) { }

        public override ConversationState GetNextStateFromMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(GetNextStateFromMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                case StockStreamResponseMessage m:
                    var stockHistory = m.RecentHistory;
                    Log.Info($"Received stock stream response with {stockHistory.Count} days of recent trading.");
                    for (int i=0; i<5 && i < stockHistory.Count; i++)
                    {
                        Log.Info(stockHistory[i].ToString());
                    }
                    nextState = new EndConversationState(ConversationID);
                    break;
                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");
                    nextState = new EndConversationState(ConversationID);
                    break;
                default:
                    Log.Error($"No logic to process incoming message of type {incomingMessage.Contents?.GetType()}.");
                    Log.Error($"Ending conversation {ConversationID}.");
                    nextState = new EndConversationState(ConversationID);
                    break;
            }

            Log.Debug($"{nameof(GetNextStateFromMessage)} (exit)");
            return nextState;
        }
        
        public override void OnStateEnd()
        {
            Log.Debug($"{nameof(OnStateEnd)} (enter)");

            //Do nothing

            Log.Debug($"{nameof(OnStateEnd)} (enter)");
        }

        public override void OnStateStart()
        {
            Log.Debug($"{nameof(OnStateStart)} (enter)");

            //TODO: Use Post Office to send request message to StockServer
            Log.Info("InitiateStockStreamRequestState OnStateStart running");

            Log.Debug($"{nameof(OnStateStart)} (enter)");
        }
    }
}
