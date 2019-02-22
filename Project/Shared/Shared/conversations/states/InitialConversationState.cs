using System;
using Shared.Comms.Messages;

namespace Shared.Conversations.States
{
    public class InitialConversationState : ConversationState
    {
        public InitialConversationState(Conversation owner) : base(owner) { }
            
        public override ConversationState ProcessMessage(Message newMessage)
        {
            throw new NotImplementedException();
        }
    }
}
