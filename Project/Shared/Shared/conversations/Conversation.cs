using log4net;
using Shared.Comms.MailService;
using System;

namespace Shared.Conversations
{
    public abstract class Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Conversation(string conversationId)
        {
            if(string.IsNullOrEmpty(conversationId))
            {
                throw new NullReferenceException();
            }
            else
            {
                ConversationId = conversationId;

                //We may want to move this AddConverstion call out of this constructor. I put it here for time sake.
                //-Dsphar 2/22/19
                ConversationManager.AddConversation(this);
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

        private ConversationState CurrentState;
        public DateTime LastUpdateTime{get; private set;}
        public readonly string ConversationId;

        public void UpdateState(Envelope incomingEnvelope)
        {
            Log.Debug(string.Format("Enter - {0}", nameof(UpdateState)));

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

            Log.Debug(string.Format("Exit - {0}", nameof(UpdateState)));
        }
    }
}
