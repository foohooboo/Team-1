using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;

namespace Shared.Conversations.SharedStates
{
    class Template_ConvState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Template_ConvState(int processNum) : base(ConversationManager.GenerateNextId(processNum)) { }

        public override ConversationState GetNextStateFromMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(GetNextStateFromMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                //TODO: Add state-specific message handling here. Given the incoming message,
                //you should set nextState to the next ConversationState expected in the conversation.
                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");
                    nextState = new ConversationDoneState(ConversationID, this);
                    break;
                default:
                    Log.Error($"No logic to process incoming message of type {incomingMessage.Contents?.GetType()}.");
                    Log.Error($"Forcing conversation {ConversationID} into done state.");
                    nextState = new ConversationDoneState(ConversationID, this);
                    break;
            }

            Log.Debug($"{nameof(GetNextStateFromMessage)} (exit)");
            return nextState;
        }

        public override void OnStateStart()
        {
            Log.Debug($"{nameof(OnStateStart)} (enter)");

            //TODO: Add any logic this state needs to perform when first started.
            //This logic is likely building and sending a message.

            Log.Debug($"{nameof(OnStateStart)} (exit)");
        }

        //OPTIONAL: function to add logic when state is ending. Default is do nothing.
        //public override void OnStateEnd() { }

        //OPTIONAL: function to handle a state timeout event. Default is to
        //re-call the OnStateStart method up to the configured number of retries,
        //then force the conversation into the done state.
        //public override void HandleTimeout() { }
    }
}
