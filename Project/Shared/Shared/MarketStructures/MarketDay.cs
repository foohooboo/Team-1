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

                bool alreadyHasCash = _tradedCompanies.Any(c => c.Symbol != null && c.Symbol.Equals("$"));
                if (!alreadyHasCash){
                    var dollarAsStock = new ValuatedStock()
                    {
                        Symbol = "$",
                        Name = "US Dollars",
                        Close = 1
                    };
                    _tradedCompanies.Add(dollarAsStock);
                }
            }
        }

        public MarketDay(string date)
        {
            Date = date;
            TradedCompanies = new List<ValuatedStock> ();
        }

        public MarketDay(string date, ValuatedStock[] starterArray)
        {
            Date = date;
            TradedCompanies = starterArray.Cast<ValuatedStock>().ToList();
        }

        /// <summary>
        /// Do not use this default constructor in code.
        /// We have to have it for the JSON serializer.
        /// Use the other available constructors instead.
        /// </summary>
        public MarketDay() {
            
        }
    }
}
