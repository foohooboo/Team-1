using System.Collections.Generic;
using Shared.MarketStructures;

namespace Shared.Comms.Messages
{
    public class StockStreamResponseMessage : Message
    {
        public MarketSegment RecentHistory
        {
            get; set;
        }

        public StockStreamResponseMessage() 
        {
            RecentHistory = new MarketSegment();
        }
    }
}