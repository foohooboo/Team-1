using log4net;
using Shared.comms.messages;
using Shared.conversations.states;
using System;

namespace Shared.conversations
{
    public class Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Id { get; private set; }

        public Conversation(string id)
        {
            Log.Debug(string.Format("Enter - {0}", nameof(Conversation)));

            Id = id;
            CurrentState = initialState;
            LastUpdateTime = DateTime.Now;

            Log.Debug(string.Format("Exit - {0}", nameof(Conversation)));
        }

        internal void End()
        {
            throw new NotImplementedException();
        }

        private ConversationState CurrentState;
        public DateTime LastUpdateTime{get; private set;}

        public void UpdateState(Message newMessage)
        {
            Log.Debug(string.Format("Enter - {0}", nameof(UpdateState)));

            var nextState = CurrentState.ProcessMessage(newMessage);
            
            //Note: The logic below assumes state will always change with a valid incoming message (as is the case for our project).
            //If we ever add a case where it is expected state will not change with an incoming message, we need to change this
            // logic to refresh the LastUpdateTime without a changing state. Otherwise conversations may falsely timeout.
            //-Dsphar 2/21/19
            if (nextState != CurrentState)
            {
                CurrentState = nextState;
                LastUpdateTime = DateTime.Now;
            }

            Log.Debug(string.Format("Exit - {0}", nameof(UpdateState)));
        }
    }
}
