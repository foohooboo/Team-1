using System.Collections.Generic;

namespace Shared.Comms.Messages
{
    public class UpdateLeaderBoardMessage : Message
    {
        public UpdateLeaderBoardMessage()
        {

        }

        public Dictionary<string, double> Records
        {
            get; set;
        }
    }
}
