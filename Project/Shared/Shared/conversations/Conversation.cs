using log4net;
using Shared.Comms.MailService;
using Shared.Conversations.SharedStates;
using System;

namespace Shared.Conversations
{
    public abstract class Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConversationState CurrentState { get; private set; }
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
            }
        }

        public void StartConversation()
        {
            if (ConversationStarted)
            {
                Log.Error("Cannot start conversation more than once.");
            }
            else if (CurrentState == null)
            {
                Log.Error("Cannot start conversation without setting an initial state.");
            }
            else
            {
                LastUpdateTime = DateTime.Now;
                CurrentState.Send();
                ConversationStarted = true;
            }
        }

        public void UpdateState(Envelope incomingEnvelope)
        {
            Log.Debug($"{nameof(UpdateState)} (enter)");

            var nextState = CurrentState.HandleMessage(incomingEnvelope);
            if (nextState != null)
            {
                UpdateState(nextState);
            }
            else
            {
                Log.Warn($"Cannot create next {ConversationId} conversation state from message {incomingEnvelope.Contents.MessageID}.");
            }

            Log.Debug($"{nameof(UpdateState)} (exit)");
        }

        public void UpdateState(ConversationState nextState)
        {
            Log.Debug($"{nameof(UpdateState)} (enter)");
            
            if (nextState != null)
            {
                LastUpdateTime = DateTime.Now;
                CurrentState.Cleanup();
                CurrentState = nextState;
                CurrentState.Prepare();
                CurrentState.Send();
            }
            else
            {
                Log.Warn($"Cannot update conversation to a null state.");
            }

            Log.Debug($"{nameof(UpdateState)} (exit)");
        }

        public void HandleTimeout()
        {
            if (!(CurrentState is ConversationDoneState))
            {
                Log.Warn($"Raising timeout event for Conversation {ConversationId}.");
            }
            LastUpdateTime = DateTime.Now;
            CurrentState.HandleTimeout();
        }
    }
}
