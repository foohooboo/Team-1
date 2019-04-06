using log4net;
using Shared.Comms.ComService;
using Shared.Conversations.SharedStates;
using System.Net;

namespace Shared.Conversations
{
    public abstract class ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly Conversation Conversation;
        public Envelope OutboundMessage { get; private set; }
        protected int CountRetrys;
        public readonly ConversationState PreviousState = null;
        private bool StatePrepared = false;

        protected readonly string MessageId = "";
        protected IPEndPoint To = null;

        /// <summary>
        /// Use this constructor if state was created by a local action. Do NOT use this
        /// constructor for states which are created from an incoming message.
        /// </summary>
        /// <param name="conversation"></param>
        public ConversationState(Conversation conversation, ConversationState previousState)
        {
            Conversation = conversation;
            PreviousState = previousState;
        }

        /// <summary>
        /// Use this constructor for states created by an incoming message. Do NOT use this
        /// constructor for states which are created from locally triggered events/actions.
        /// </summary>
        /// <param name="conversation"></param>
        public ConversationState(Envelope env, Conversation conversation, ConversationState parentState) : this(conversation, parentState)
        {
            MessageId = env.Contents.MessageID;
            To = env.To;
        }

        /// <summary>
        /// Prepare and return an envelope that this state will send. If this state does not need to send an envelope, return null.
        /// This method gets called in the state's base constructor.
        /// method.
        /// </summary>
        public abstract Envelope Prepare();

        /// <summary>
        /// Incoming message handler. Define the logic required to take an incoming message, determine the next state, 
        /// and return a new instance of that state. Return null if current state does not expect the received message type. 
        /// Note: You may copy Template_ConvState for general method structure.
        /// </summary> 
        /// <param name="newMessage"></param>
        /// <returns></returns>
        public abstract ConversationState HandleMessage(Envelope incomingMessage);

        /// <summary>
        /// Method used to handle a timeout event. Note: default behavior is to re-send the 
        /// pre-prepared envelope as many times as defined in the program configuration file.
        /// After that, this handler ends the conversation. 
        /// You may override this method if you want different behavior.
        /// </summary>
        public virtual void HandleTimeout()
        {
            if (++CountRetrys <= Config.GetInt(Config.DEFAULT_RETRY_COUNT))
            {
                Log.Warn($"Initiating retry for conversation {Conversation.Id}.");
                Send();
            }
            else
            {
                Log.Warn($"Timeout event is forcing conversation {Conversation.Id} into the Done state.");
                Conversation.UpdateState(new ConversationDoneState(Conversation, this));
            }
        }

        /// <summary>
        /// Override this function if a given conversation state needs to do some sort of cleanup
        /// during the end of its life.
        /// </summary>
        public virtual void Cleanup()
        {

        }

        /// <summary>
        /// Actual handler for incoming messages. First it checks to see if the incoming message was already handled.
        /// If it was, it returns self to ensure the OnHandle isn't called again for the already processed message. 
        /// If the incoming message is not recognized as already-handled, it tries to handle it itself. 
        /// If current state can't handle the incoming message, it may try to have it's parent handle the same message.
        /// </summary>
        /// <param name="incomingMessage">The message to be processed</param>
        /// <param name="haveParentTryOnFailure">
        /// If current state cannot process the message, should it try to have its parent state handle the message?
        /// External callers of this method should set this to true. This results in only a single ancestor (the direct parent)
        /// handling an unrecognized, unhandled message.
        /// </param>
        /// <returns></returns>
        public virtual ConversationState OnHandleMessage(Envelope incomingMessage, int numAncestorsTryOnFailure)
        {
            ConversationState nextState = null;
            
            if (!string.IsNullOrEmpty(MessageId) && incomingMessage.Contents.MessageID.Equals(MessageId))
            {
                //If the incoming message is a repeat of one already processed (resulted in the creation of this state), do nothing.
                //The send will get re-called without calling OnPrepare again.
                nextState = this;
            }
            else
            {
                //Incoming message not recognized. Try to handle it here
                nextState = HandleMessage(incomingMessage);

                if (nextState == null && numAncestorsTryOnFailure>0)
                {
                    //current state couldn't handle incoming message, see if parent can handle it.
                    nextState = PreviousState?.OnHandleMessage(incomingMessage, --numAncestorsTryOnFailure);
                }
            }

            return nextState;
        }

        public virtual void DoPrepare()
        {
            if (!StatePrepared)
            {
                OutboundMessage = Prepare();
                StatePrepared = true;
            }
        }
               

        public virtual void Send()
        {
            Log.Debug($"{nameof(Send)} (enter)");
            if (OutboundMessage != null)
            {
                ComService.Send(Config.DEFAULT_UDP_CLIENT, OutboundMessage);
            }
            Log.Debug($"{nameof(Send)} (exit)");
        }
    }
}