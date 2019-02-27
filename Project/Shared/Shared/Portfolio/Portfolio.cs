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

        //private Dictionary<string, Asset> Assets;

        public Dictionary<string, Asset> Assets
        {
            get;set;
        }

        //Add/Remove an Asset object in the Portfolio
        //If the specific Asset already exists within the portfolio, increment/decrement the Quantity
        //ass quantity can be < 0 for removal of Assets
        //fails silently
        //These two methods could maybe take a stock symbol and a quantity as parameters instead of an Asset object
        public void ModifyAsset(Asset ass)
        {
            if(Assets.ContainsKey(ass.RelatedStock.Symbol))
            {
                Assets[ass.RelatedStock.Symbol].Quantity += ass.Quantity;

                if (Assets[ass.RelatedStock.Symbol].Quantity <= 0)
                {
                    Assets.Remove(ass.RelatedStock.Symbol);
                }
            }
            else if(ass.Quantity > 0)
            {
                Assets.Add(ass.RelatedStock.Symbol, ass);
            }
        }
    }
}
