using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Comms.Messages
{
    public static class MessageFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly JsonSerializerSettings settings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.All
        };

        private static readonly string ProjectName = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name;

        public static int MessageCount
        {
            get; private set;
        }

        public static Message GetMessage(byte[] bytes, bool scrubJson = true)
        {
            string readData = Encoding.Default.GetString(bytes);
            return (GetMessage(readData, scrubJson));
        }

        public static Message GetMessage(string json, bool scrubJson = true)
        {
            Log.Debug($"{nameof(GetMessage)} (enter)");

            Message message = null;

            try
            {
                message = JsonConvert.DeserializeObject<Message>(json, settings);
            }
            catch (Exception e)
            {
                Log.Error("Error when deserializing message.");
                Console.WriteLine($"Error when deserializing message...\n{e}");
                Log.Error(e.Message);
                Log.Error(e.StackTrace);
            }

            Log.Debug($"{nameof(GetMessage)} (exit)");
            return message;
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
