using log4net;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Shared.Comms.Messages
{
    public static class MessageFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            Log.Debug($"{nameof(GetMessage)} (enter)");

            var v = JsonConvert.DeserializeObject<Message>(json, settings);

            Log.Debug($"{nameof(GetMessage)} (exit)");
            return v;
        }

        public static Message GetMessage<TMessage>(int processID, int portfolioID)
        {
            Log.Debug($"{nameof(GetMessage)} (enter)");

            var message = Activator.CreateInstance(typeof(TMessage)) as Message;
            message.MessageID = $"{processID}-{portfolioID}-{GetNextMessageCount()}";

            Log.Debug($"{nameof(GetMessage)} (exit)");
            return message;
        }

        public static int GetNextMessageCount()
        {
            return ++MessageCount;
        }
    }
}
