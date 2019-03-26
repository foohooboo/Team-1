using log4net;
using Shared.Comms.MailService;
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
        protected ConversationState PreviousState = null;

        protected readonly string MessageId = "";
        protected readonly IPEndPoint To = null;

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
                ConversationManager.GetConversation(Conversation.Id).UpdateState(new ConversationDoneState(Conversation, this));
            }
        }

        /// <summary>
        /// Override this function if a given conversation state needs to do some sort of cleanup
        /// during the end of its life.
        /// </summary>
        public virtual void Cleanup()
        {

        }

        public virtual ConversationState OnHandleMessage(Envelope incomingMessage)
        {
            ConversationState nextState = null;
            
            if (!string.IsNullOrEmpty(MessageId) && incomingMessage.Contents.MessageID.Equals(MessageId))
            {
                //If the incoming message is a repeat of one already processed, do nothing.
                //The already prepared message will be re-sent by conversation manager.
                nextState = this;
            }
            else
            {
                //Incoming message not recognized. Try to handle it here
                nextState = HandleMessage(incomingMessage);

                if (nextState == null)
                {
                    //current state couldn't handle incoming message, see if parent can handle it.
                    nextState = PreviousState?.HandleMessage(incomingMessage);
                }
            }

            return nextState;
        }

        public virtual void DoPrepare()
        {
            if (OutboundMessage == null) //Only do this once. If Env not null, this method has already been called, do nothing.
            {
                OutboundMessage = Prepare();
            }
        }
               

        public virtual void Send()
        {
            if (OutboundMessage != null)
            {
                PostOffice.Send(OutboundMessage);
            }
        }
    }
}
