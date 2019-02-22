using Newtonsoft.Json;

namespace Shared.Comms.Messages
{
    public abstract class Message
    {
        public Message()
        {

        }

        public string ConversationID
        {
            get; set;
        }

        public int SourceID
        {
            get; set;
        }

        public string MessageID
        {
            get; set;
        }

        public string Encode()
        {
            return JsonConvert.SerializeObject(this, MessageFactory.settings);
        }
    }
}