using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.comms.messages
{
    public class AckMessage : Message
    {
        //Constructors
        public AckMessage() : base(MessageTypes.Ack)
        {
            AckHello = "Ack says Hello!!";
        }

        public AckMessage(JObject jObj) : base(MessageTypes.Ack)
        {
            AckHello = jObj["AckHello"].ToString();
        }

        public string AckHello;
    }
}
