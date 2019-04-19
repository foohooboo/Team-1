using System.Collections.Generic;
using Shared.MarketStructures;

namespace Shared.PortfolioResources
{
    public class Asset
    {
        public static Asset GetTestAsset()
        {
            return new Asset()
            {
                RelatedStock = Stock.GetTestStock(),
                Quantity = 500
            };
        }

        public Asset()
        {
            RelatedStock = new Stock();
        }

        public Asset(Stock relatedStock, float quantity)
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

        public float Quantity
        {
            get; set;
        }

        public override bool Equals(object obj)
        {
            var asset = obj as Asset;
            return asset != null &&
                   EqualityComparer<Stock>.Default.Equals(RelatedStock, asset.RelatedStock) &&
                   Quantity == asset.Quantity;
        }

        public override int GetHashCode()
        {
            var hashCode = 300933774;
            hashCode = hashCode * -1521134295 + EqualityComparer<Stock>.Default.GetHashCode(RelatedStock);
            hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
            return hashCode;
        }
    }
}