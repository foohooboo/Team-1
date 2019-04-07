using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations.SharedStates;

namespace Shared.Conversations.SendErrorMessage
{
    public class SendErrorMessageState : ConversationState
    {

        private string ErrorText;
        private int ProcessId;

        public SendErrorMessageState(string errorText, Envelope env, Conversation conversation, ConversationState parentState, int processId) : base(env, conversation, parentState)
        {
            ErrorText = errorText;
            ProcessId = processId;
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            //Sent errors do not expect any incoming messages
            return null;
        }

        public override Envelope Prepare()
        {
            var mes = MessageFactory.GetMessage<ErrorMessage>(ProcessId, 0) as ErrorMessage;
            mes.ConversationID = Conversation.Id;
            mes.ErrorText = ErrorText;

            var env = new Envelope(mes)
            {
                To = this.To
            };
            return env;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
        }
    }
}
