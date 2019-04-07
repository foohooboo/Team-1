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

        public byte[] Encode()
        {
            var jsonData = JsonConvert.SerializeObject(this, MessageFactory.settings);

            return System.Text.Encoding.Default.GetBytes(jsonData);
        }
    }
}