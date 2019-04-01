using System.Collections.Generic;
using System.Linq;

namespace Shared.MarketStructures
{
    /// <summary>
    /// Is a List<ValuatedStock> which represents a single day of trading.
    /// </summary>
    public class MarketDay
    {
        public string Date { get; set; }

        private List<ValuatedStock> _tradedCompanies;
        public List<ValuatedStock> TradedCompanies
        {
            get => _tradedCompanies;
            set {
                _tradedCompanies = value;

                //Add dollar
                var dollarAsStock = new ValuatedStock()
                {
                    Symbol = "$",
                    Name = "US Dollars",
                    Close = 1
                };
                _tradedCompanies.Add(dollarAsStock);
            }
        }

        public MarketDay(string date)
        {
            Date = date;
        }

        public MarketDay(string date, ValuatedStock[] starterArray)
        {
            Date = date;
            TradedCompanies = starterArray.Cast<ValuatedStock>().ToList();
        }

        public MarketDay() { }
    }
}
