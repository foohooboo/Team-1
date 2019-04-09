using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.MarketStructures
{
    /// <summary>
    /// Is a List<MarketDay> which represents multiple days of trading in consecutive order.
    /// </summary>
    public class MarketSegment : List<MarketDay>
    {
        public MarketSegment()
        {
            
        }

        public MarketSegment(MarketSegment start)
        {
            foreach(var day in start)
            {
                var dayCopy = new MarketDay(day);
                this.Add(dayCopy);
            }
        }
    }
}
