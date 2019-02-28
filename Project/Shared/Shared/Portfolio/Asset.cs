namespace Shared.Portfolio
{
    public class Asset
    {
        public Asset()
        {

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
