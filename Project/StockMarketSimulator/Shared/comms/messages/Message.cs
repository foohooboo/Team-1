using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.comms.messages
{
    public abstract class Message
    {
        //TODO: Message creation still needs to be forced through the MessaegFactory.
        //I haven't gotten to it yet. Current state was made to build message base class.



        public Message()
        {

        }

        public string MessageID
        {
            get; set;
        }

        //Public Methods
        public string Encode()
        {
            return JsonConvert.SerializeObject(this, MessageFactory.settings);
        }

        //Protected Methods

        //Private Methods
    }
}