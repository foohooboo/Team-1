using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.comms.messages
{
    public abstract class Message
    {
        public Message()
        {

        }

        public string MessageID
        {
            get; set;
        }

        public string ConversationId
        {
            get; set;
        }

        public string Encode()
        {
            return JsonConvert.SerializeObject(this, MessageFactory.settings);
        }
    }
}