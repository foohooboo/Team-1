using Shared.MarketStructures;

namespace Shared.Portfolio
{
    public class Asset
    {
        public Asset()
        {
            RelatedStock = new Stock();
        }

        public Asset(Stock relatedStock, int quantity)
        {
            RelatedStock = relatedStock;
            Quantity = quantity;
        }

        public Asset(Asset asset)
        {
            RelatedStock = asset.RelatedStock;
            Quantity = asset.Quantity;
        }

        public Stock RelatedStock
        {
            get; set;
        }

        public int Quantity
        {
            get; set;
        }
    }
}
