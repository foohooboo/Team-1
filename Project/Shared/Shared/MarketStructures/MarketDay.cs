using System.Collections.Generic;

namespace Shared.MarketStructures
{
    /// <summary>
    /// Is a List<ValuatedStock> which represents a single day of trading.
    /// </summary>
    public class MarketDay: List<ValuatedStock>
    {
        public string Date { get; set; }

        public MarketDay(string date)
        {
            Date = date;
        }

        public MarketDay(string date, ValuatedStock[] starterArray)
        {
            Date = date;
            AddRange(starterArray);
        }

        public MarketDay() { }
    }
}
