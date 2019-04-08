using System.Collections;
using System.Collections.Generic;

namespace Shared.Comms.Messages
{
    public class UpdateLeaderBoardMessage : Message
    {
        public UpdateLeaderBoardMessage()
        {
            Records = new SortedList<float,string>();
        }

        /*
         * NOTE: -- System.Collections.Generic.SortedList lets us explicitly define expected types
         * -- System.Collections.SortedList does not seem to allow that
         * -- We are expecting the SortedList to hold (key,value) pairs of (portfolio-value, username)
         * -- Whether you give it (string, string) or (double, string) pairs, it will all come out of the serializer 
         * as (string, string) pairs. 
         * -- Now you know, and knowing is half the battle. GO JOE!
         */ 
        public SortedList<float,string> Records
        {
            get; set;
        }
    }
}
