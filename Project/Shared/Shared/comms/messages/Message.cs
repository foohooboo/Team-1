using Newtonsoft.Json;

namespace Shared.Comms.Messages
{
    public abstract class Message
    {
        public Message()
        {

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