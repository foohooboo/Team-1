using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.comms.messages
{
    public class AckMessage : Message
    {
        public AckMessage() : base()
        {

        }

        public string AckHello;
    }
}
