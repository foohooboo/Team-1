using log4net;
using Shared.Comms.MailService;
using Shared.Conversations.SharedStates;

namespace Shared.Conversations
{
    public abstract class ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly Conversation ParentConversation;
        public Envelope Env { get; private set; }
        protected int CountRetrys;

        public ConversationState(Conversation conversation)
        {
            ParentConversation = conversation;
        }

        /// <summary>
        /// Method used to handle a timeout event. Note: default behavior is to re-send the 
        /// Prepared() envelope as many times as defined in the program configuration file.
        /// After that, this handler ends the conversation. 
        /// You may override this method if you want different behavior for a state.
        /// </summary>
        public virtual void HandleTimeout()
        {
            if(++CountRetrys <= Config.GetInt(Config.DEFAULT_RETRY_COUNT))
            {
                Log.Warn($"Initiating retry for conversation {ParentConversation.Id}.");
                Send();
            }
            else
            {
                Log.Warn($"Timeout event is forcing conversation {ParentConversation.Id} into the Done state.");
                ConversationManager.GetConversation(ParentConversation.Id).UpdateState(new ConversationDoneState(ParentConversation, this));
            }
        }

        /// <summary>
        /// Incoming message handler. Define the logic required to take an incoming message, determine the next state, and return a new instance
        /// of that state. Return null if current state does not expect the received message type. 
        /// Note: You may copy Template_ConveState for general method structure.
        /// </summary> 
        /// <param name="newMessage"></param>
        /// <returns></returns>
        public abstract ConversationState HandleMessage(Envelope newMessage);

        /// <summary>
        /// Prepare and return an envelope that this state will send. If this state does not need to send an envelope, return null.
        /// This method gets called in the state's base constructor.
        /// method.
        /// </summary>
        public abstract Envelope Prepare();

        public virtual void DoPrepare() { Env = Prepare(); }
        public virtual void DoPrepare(string test) { Env = Prepare(); }

        /// <summary>
        /// Override this function if a given conversation state needs to do some sort of cleanup
        /// during the end of its life.
        /// </summary>
        public virtual void Cleanup() {
        
        }

        public virtual void Send()
        {
            if (Env != null)
            {
                PostOffice.Send(Env);
            }
        }
    }
}
