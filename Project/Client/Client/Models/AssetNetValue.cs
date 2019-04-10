namespace Client.Models
{
    public class AssetNetValue
    {
        public string Symbol { get; private set; }
        public string Quantity { get; private set; }
        public string TotalValue { get; private set; }

        public AssetNetValue(string symbol, string quantity, string totalValue)
        {
            Symbol = symbol;
            Quantity = quantity;
            TotalValue = totalValue;
        }
    }
}
