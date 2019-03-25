using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;

namespace Shared.Conversations.SharedStates
{
    class Template_ConvState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Template_ConvState(Conversation conv) : base(conv) { }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                //TODO: Add state-specific message handling here. Given the incoming message,
                //you should set nextState to the next ConversationState expected in the conversation.
                case ErrorMessage m:
                    Log.Error($"Received error message as reply...\n{m.ErrorText}");
                    nextState = new ConversationDoneState(ParentConversation, this);
                    break;
                default:
                    Log.Error($"No logic to process incoming message of type {incomingMessage.Contents?.GetType()}. Ignoring message.");
                    break;
            }

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return nextState;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            Envelope env = null;

            //TODO: Add any logic this state needs to perform when first started.
            //If this state is going to send a message to another process, set
            //the env variable.

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }

        //OPTIONAL: function to add logic when state is ending. Default is do nothing.
        //public override void OnStateEnd() { }

        //OPTIONAL: function to handle a state timeout event. Default is to
        //re-call the Send method up to the configured number of retries,
        //then force the conversation into the done state.
        //public override void HandleTimeout() { }
    }
}
