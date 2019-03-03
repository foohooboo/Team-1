﻿using System.Collections.Generic;

namespace Shared.MarketStructures
{
    /// <summary>
    /// Is a List<ValuatedStock> which represents a single day of trading.
    /// </summary>
    public class MarketDay
    {
        public string Date { get; set; }
        public List<ValuatedStock> Data = new List<ValuatedStock>();

        public MarketDay(string date)
        {
            Date = date;
        }

        public MarketDay(string date, ValuatedStock[] starterArray)
        {
            Date = date;
            Data.AddRange(starterArray);
        }

        public MarketDay() { }
    }
}
