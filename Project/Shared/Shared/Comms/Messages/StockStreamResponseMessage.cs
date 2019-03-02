using System.Collections.Generic;
using Shared.MarketStructures;

namespace Shared.Comms.Messages
{
    public class StockStreamResponseMessage : Message
    {
        public StockMarketDay StockMarketDayList
        {
            get; set;
        }

        public StockStreamResponseMessage()
        {
            StockMarketDayList = new StockMarketDay();
        }
    }
}