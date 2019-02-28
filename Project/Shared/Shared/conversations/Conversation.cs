using log4net;
using Shared.Comms.MailService;
using System;

namespace Shared.Conversations
{
    public abstract class Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ConversationState CurrentState;
        public readonly string ConversationId;
        public bool ConversationStarted { get; private set; }
        public DateTime LastUpdateTime { get; private set; }

        public Conversation(string conversationId)
        {
            if(string.IsNullOrEmpty(conversationId))
            {
                throw new NullReferenceException();
            }
            else
            {
                ConversationId = conversationId;
            }
        }

        public void SetInitialState(ConversationState state)
        {
            if (CurrentState != null)
            {
                Log.Error("Cannot set initial conversation state more than once.");
            }
            else
            {
                LastUpdateTime = DateTime.Now;
                CurrentState = state;
                CurrentState.OnStateStart();
            }
        }

        public void StartConversation()
        {
            if (ConversationStarted)
            {
                Log.Error("Cannot start conversation more than once.");
            }
            else
            {
                LastUpdateTime = DateTime.Now;
                CurrentState.OnStateStart();
                ConversationStarted = true;
            }
        }

        public void UpdateState(Envelope incomingEnvelope)
        {
            Log.Debug($"{nameof(UpdateState)} (enter)");

            var nextState = CurrentState.GetNextStateFromMessage(incomingEnvelope);
            if (nextState != null)
            {
                LastUpdateTime = DateTime.Now;
                CurrentState.OnStateEnd();
                CurrentState = nextState;
                CurrentState.OnStateStart();
            }
            else
            {
                Log.Warn($"Unable to advance conversation to next state with message {incomingEnvelope.Contents.MessageID}.");
            }

            Log.Debug($"{nameof(UpdateState)} (exit)");
        }
    }
}
