using log4net;
using System.Collections.Generic;

namespace Shared.PortfolioResources
{
    public class Portfolio
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //TODO: It might be good to break this class into two. 
        //1- A readonly portfolio with name, password, and assets.
        //2- A managed portfolio, which extends the base Portfolio, but can also modify the assets, etc.
        //-Dsphar 4/10/2019

        public Portfolio()
        {
            
        }

        public Portfolio(Portfolio original)
        {
            PortfolioID = original.PortfolioID;
            Username = original.Username;
            Password = original.Password;
            Assets = original.CloneAssets();
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

        public bool WriteAuthority
        {
            get; set;
        }

        private object LockAssets = new object();
        private readonly Dictionary<string, Asset> _assets = new Dictionary<string, Asset>();
        public Dictionary<string, Asset> Assets
        {
            get
            {
                //return a copy of the owned assets to prevent accidental changes
                Dictionary<string, Asset> assets = null;
                lock (LockAssets)
                {
                    assets = CloneAssets();
                }
                return assets;
            }
            private set
            { 
                //never set the asset directly, always use the ModifyAsset function
            }
        }

        /// <summary>
        /// Add(+qty) or remove(-qty) the desired quantity stock with the provided symbol.
        /// Do nothing and return false if entire transaction cannot be completed (insufficient funds, etc).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="quantity"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool ModifyAsset(string symbol, int quantity, out string errorMessage)
        {
            errorMessage = "";
            if (quantity == 0)
            {
                errorMessage = "You cannot make a transaction for 0 stocks.";
                return false;
            }

            lock (LockAssets)
            {

                //Buying
                if (quantity > 0)
                {

                }

                //Selling
                else
                {
                    if (!Assets.ContainsKey(symbol))
                    {
                        errorMessage = $"You do not own any {symbol} stocks.";
                        return false;
                    }

                    if
                }
            }




            if (Assets.ContainsKey(asset.RelatedStock.Symbol))
            {
                Assets[asset.RelatedStock.Symbol].Quantity += asset.Quantity;

                if (Assets[asset.RelatedStock.Symbol].Quantity <= 0)
                {
                    Assets.Remove(asset.RelatedStock.Symbol);
                }
            }
            else if (asset.Quantity > 0)
            {
                Assets.Add(asset.RelatedStock.Symbol, asset);
            }

            Log.Debug($"{nameof(ModifyAsset)} (exit)");
        }

        public Asset GetAsset(string symbol)
        {
            Log.Debug($"{nameof(GetAsset)} (enter)");

            if (Assets.TryGetValue(symbol, out Asset asset))
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

        public Dictionary<string, Asset> CloneAssets()
        {
            return new Dictionary<string, Asset>(_assets);
        }
    }
}