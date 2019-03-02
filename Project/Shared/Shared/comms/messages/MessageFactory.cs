using Newtonsoft.Json;
using System;
using System.Text;

namespace Shared.Comms.Messages
{
    public static class MessageFactory
    {
        public static readonly JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public static int MessageCount
        {
            get; private set;
        }

        public static Message GetMessage(byte[] bytes)
        {
            string readData = Encoding.Default.GetString(bytes);

            return (GetMessage(readData));
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
