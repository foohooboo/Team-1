using System;
using System.Collections.Generic;
using System.Text;
using Shared.comms.messages;

namespace Shared.conversations.states
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
