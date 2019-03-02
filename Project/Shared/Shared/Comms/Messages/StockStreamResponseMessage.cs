using System.Collections.Generic;
using Shared.MarketStructures;

namespace Shared.Comms.Messages
{
    public class StockStreamResponseMessage : Message
    {
        public MarketDay MarketDayList
        {
            get; set;
        }

        public StockStreamResponseMessage()
        {
            MarketDayList = new MarketDay();
        }
    }
}