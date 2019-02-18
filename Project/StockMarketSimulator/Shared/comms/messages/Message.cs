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


        //Abstract & Virtual Members/Methods
        public readonly MessageTypes MType;

        //Constructors
        protected Message(MessageTypes messageType)
        {
            MType = messageType;
        }

        //Public Methods
        public string Encode()
        {
            return JsonConvert.SerializeObject(this);
        }

        //Protected Methods

        //Private Methods
    }
}
