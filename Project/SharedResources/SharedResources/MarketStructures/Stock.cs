using System;
using System.Collections.Generic;
using SharedResources.DataGeneration;

namespace Shared.MarketStructures
{
    [Serializable()]
    public class Stock
    {
        public string Symbol
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }

        public static Stock GetTestStock()
        {
            return new Stock()
            {
                Symbol = DataGenerator.GetRandomString(3),
                Name = DataGenerator.GetRandomString(7)
            };
        }

        public Stock()
        {
            Symbol = null;
            Name = null;
        }

        public Stock(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }

        public Stock(Stock s)
        {
            Symbol = s.Symbol;
            Name = s.Name;
        }

        public override bool Equals(object obj)
        {
            var stock = obj as Stock;

            return stock != null &&
                   Symbol == stock.Symbol &&
                   Name == stock.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = -1492472673;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Symbol);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}