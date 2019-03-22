using Shared;
using Shared.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Conversations
{
    public class InitiateTransactionConversation : Conversation
    {
        public InitiateTransactionConversation() : base(Config.GetInt(Config.CLIENT_PROCESS_NUM))
        {

        }
    }
}
