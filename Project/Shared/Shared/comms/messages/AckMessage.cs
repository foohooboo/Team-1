using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.Comms.Messages
{
    public class AckMessage : Message
    {
        public AckMessage() : base()
        {

        }

        public string AckHello;
    }
}
