using System.Collections.Generic;
using Shared.MarketStructures;

namespace Shared.Comms.Messages
{
    public class StockHistoryResponseMessage : Message
    {
        public MarketSegment RecentHistory
        {
            get; set;
        }

        public StockHistoryResponseMessage() 
        {
            RecentHistory = new MarketSegment();
        }
    }
}