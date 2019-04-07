using log4net;
using Newtonsoft.Json;
using System;
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

            //Note: The following hack was added because we are using the Newtonsoft serializer on different projects.
            //It adds the project name to the Type field in JSON, and this causes an exception to be thrown when
            //decoding json from projects with a different name. We might be able to replace this hack with a setting
            //that allows cross-project serialization, I just couldn't find that setting after a 20 minute Google-fu
            //session. Hence I decided to hack this before the assignment is due.
            //-dsphar 3/3/2019
             if (scrubJson)
            {
                json = !ProjectName.Equals("Broker")? json.Replace("Broker", ProjectName) :json;
                json = !ProjectName.Equals("StockServer") ? json.Replace("StockServer", ProjectName) : json;
                json = !ProjectName.Equals("Client") ? json.Replace("Client", ProjectName) : json;
            }

            Message message = null;

            try
            {
                message = JsonConvert.DeserializeObject<Message>(json, settings);
            }
            catch (Exception e)
            {
                Log.Error("Error when deserializing message.");
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
