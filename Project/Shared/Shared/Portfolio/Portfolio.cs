using System.Collections.Generic;

namespace Shared.Portfolio
{
    class Portfolio
    {
        public Portfolio()
        {
            Assets = new Dictionary<string, Asset>();
        }

        public int PortfolioID
        {
            get; set;
        }

        public string Username
        {
            get; set;
        }

        public string Password
        {
            get; set;
        }

        public bool RequestWriteAuthority
        {
            get; set;
        }

        private Dictionary<string, Asset> Assets;

        //Add/Remove an Asset object in the Portfolio
        //If the specific Asset already exists within the portfolio, increment the Quantity
        //These two methods could maybe take a stock symbol and a quantity as parameters instead of an Asset object
        public void AddAsset(Asset ass)
        {
            if(Assets.ContainsKey(ass.RelatedStock.Symbol))
            {
                Assets[ass.RelatedStock.Symbol].Quantity += ass.Quantity;
            }
            else
            {
                Assets.Add(ass.RelatedStock.Symbol, ass);
            }
        }

        //If the Portfolio contains the Asset, decrement the quantity. 
        //If the quantity of the asset falls to 0 then remove it from the portfolio
        public void RemoveAsset(Asset ass)
        {
            if (Assets.ContainsKey(ass.RelatedStock.Symbol))
            {
                Assets[ass.RelatedStock.Symbol].Quantity -= ass.Quantity;
                if(Assets[ass.RelatedStock.Symbol].Quantity <= 0)
                {
                    Assets.Remove(ass.RelatedStock.Symbol);
                }
            }
        }
    }
}
