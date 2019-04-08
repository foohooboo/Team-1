using System;
using System.Collections;
using System.Collections.Generic;

namespace Shared.Comms.Messages
{
    public class UpdateLeaderBoardMessage : Message
    {
        public UpdateLeaderBoardMessage()
        {
            
        }



        /// <summary>
        /// The json serializer was giving me grief with the sorted lists (generic and old-school).
        /// Hence I change the message to contain a base 64 string encoding of a serialized Generic SortedList.
        /// </summary>
        public string SerializedRecords;
    }
}
