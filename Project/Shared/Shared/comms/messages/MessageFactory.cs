using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.comms.messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.comms.messages
{
    public static class MessageFactory
    {
        public static readonly JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public static int MessageCount
        {
            get; private set;
        }

        public static Message GetMessage(string json)
        {            
            return JsonConvert.DeserializeObject<Message>(json, settings);
        }

        public static Message GetMessage<TMessage>(int processID, int portfolioID)
        {
            var message = Activator.CreateInstance(typeof(TMessage)) as Message;
            message.MessageID = $"{processID}-{portfolioID}-{GetNextMessageCount()}";

            return message;
        }

        public static int GetNextMessageCount()
        {
            return ++MessageCount;
        }
    }
}
