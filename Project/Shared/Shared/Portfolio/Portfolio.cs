using log4net;
using System.Collections.Generic;

namespace Shared.Portfolio
{
    public class Portfolio
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            get; set;
        }

        //Add/Remove an Asset object in the Portfolio
        //If the specific Asset already exists within the portfolio, increment/decrement the Quantity
        //ass quantity can be < 0 for removal of Assets
        //fails silently
        //These two methods could maybe take a stock symbol and a quantity as parameters instead of an Asset object
        public void ModifyAsset(Asset asset)
        {
            if(Assets.ContainsKey(asset.RelatedStock.Symbol))
            {
                Assets[asset.RelatedStock.Symbol].Quantity += asset.Quantity;

                if (Assets[asset.RelatedStock.Symbol].Quantity <= 0)
                {
                    Assets.Remove(asset.RelatedStock.Symbol);
                }
            }
            else if(asset.Quantity > 0)
            {
                Assets.Add(asset.RelatedStock.Symbol, asset);
            }

            Log.Debug($"{nameof(ModifyAsset)} (exit)");
        }

        public Asset GetAsset(string symbol)
        {
            Log.Debug($"{nameof(GetAsset)} (enter)");

            Asset asset = null;
            if (Assets.TryGetValue(symbol, out asset))
            {
                asset = new Asset(asset);//Prepare deep copy so original can't be modified except through ModifyAsset method.
            }
            else
            {
                Log.Warn($"{Username}'s portfolio does not have an asset with symbol {symbol}");
            }
                
            Log.Debug($"{nameof(GetAsset)} (exit)");
            return asset;
        }
    }
}