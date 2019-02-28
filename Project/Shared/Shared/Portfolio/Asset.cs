namespace Shared.Portfolio
{
    public class Asset
    {
        public Asset()
        {
            RelatedStock = new Stock();
        }

        public Shared.Stock RelatedStock
        {
            get; set;
        }

        public int Quantity
        {
            get; set;
        }
    }
}
