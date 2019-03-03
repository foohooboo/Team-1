namespace Shared.MarketStructures
{
    public class Stock
    {
        public string Symbol { get; set; }
        public string Name { get; set; }

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
    }
}
