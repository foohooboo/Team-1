using System.Collections.Generic;

namespace Shared.MarketStructures
{
    /// <summary>
    /// Is a List<ValuatedStock> which represents a single day of trading.
    /// </summary>
    public class StockMarketDay: List<ValuatedStock>
    {
        public string Date { get; set; }

        public StockMarketDay(string date)
        {
            Date = date;
        }

        public StockMarketDay(string date, ValuatedStock[] starterArray)
        {
            Date = date;
            AddRange(starterArray);
        }

        public StockMarketDay() { }
    }
}
