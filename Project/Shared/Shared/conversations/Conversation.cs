using log4net;
using Shared.Comms.MailService;
using System;

namespace Shared.Conversations
{
    public abstract class Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Conversation(string id, ConversationState InitialState)
        {
            Log.Debug(string.Format("Enter - {0}", nameof(Conversation)));

            Id = id;
            CurrentState = InitialState;
            LastUpdateTime = DateTime.Now;

            Log.Debug(string.Format("Exit - {0}", nameof(Conversation)));
        }

        private ConversationState CurrentState;
        public DateTime LastUpdateTime{get; private set;}
        public string Id { get; private set; }

        public void UpdateState(Envelope incomingEnvelope)
        {
            Log.Debug(string.Format("Enter - {0}", nameof(UpdateState)));

            var nextState = CurrentState.GetNextStateFromMessage(incomingEnvelope);
                                   
            //Note: The logic below assumes state will always change with a valid incoming message (as is the case for our project).
            //If we ever add a case where it is expected state will not change with an incoming message, we need to change this
            // logic to refresh the LastUpdateTime without a changing state. Otherwise conversations may falsely timeout.
            //-Dsphar 2/21/19
            if (nextState != CurrentState)
            {
                CurrentState.OnStateEnd();
                CurrentState = nextState;
                CurrentState.OnStateStart();

                LastUpdateTime = DateTime.Now;
            }

            Log.Debug(string.Format("Exit - {0}", nameof(UpdateState)));
        }
    }
}
