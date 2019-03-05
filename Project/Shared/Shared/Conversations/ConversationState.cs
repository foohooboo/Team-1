using log4net;
using Shared.Comms.MailService;
using Shared.Conversations.SharedStates;

namespace Shared.Conversations
{
    public abstract class ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly string ConversationID;
        protected int CountRetrys;

        public ConversationState(string conversationID)
        {
            ConversationID = conversationID;
        }

        /// <summary>
        /// Method used to handle a timeout event. Note: default behavior is to re-call the 
        /// OnStateStart method as many times as defined in the program configuration file.
        /// After that this handler ends the conversation. 
        /// You may override this method if you want different behavior for a given state.
        /// </summary>
        public virtual void HandleTimeout()
        {
            if(++CountRetrys <= Config.GetInt(Config.DEFAULT_RETRY_COUNT))
            {
                Log.Warn($"Initiating retry for conversation {ConversationID}.");
                OnStateStart();
            }
            else
            {
                Log.Error($"Timeout event forced conversation {ConversationID} to end.");
                ConversationManager.GetConversation(ConversationID).UpdateState(new EndConversationState(ConversationID));
            }
            
        }

        public abstract ConversationState GetNextStateFromMessage(Envelope newMessage);
        public abstract void OnStateStart();
        public abstract void OnStateEnd();
    }
}
