using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.comms.messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.comms.messages
{
    public static class MessageFactory
    {
        public static Message GetMessage(string json)
        {
            JObject o = JObject.Parse(json);
            if (o.ContainsKey("MType"))
            {
                if (Enum.TryParse(o["MType"].ToString(), out MessageTypes type)){
                    Message m;
                    switch (type)
                    {
                        case MessageTypes.Ack:
                            m = new AckMessage(o);
                            break;
                        default:
                            throw new FormatException(string.Format("{0} message type not found.",json));
                    }
                    return m;
                }
            }
            throw new FormatException("Could not deserialize json message.");
        }
    }
}
