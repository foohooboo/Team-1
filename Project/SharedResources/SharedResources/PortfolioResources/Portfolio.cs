using System.Collections.Generic;
using System.Linq;
using log4net;
using SharedResources.DataGeneration;

namespace Shared.PortfolioResources
{
    public class Portfolio
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Portfolio GetTestPortfolio()
        {
            var asset1 = Asset.GetTestAsset();
            var asset2 = Asset.GetTestAsset();
            var asset3 = Asset.GetTestAsset();

            return new Portfolio()
            {

                PortfolioID = DataGenerator.GetRandomNumber(1),
                Username = DataGenerator.GetRandomString(6),
                Password = DataGenerator.GetRandomString(9),
                Assets = new Dictionary<string, Asset>
                {
                    {asset1.RelatedStock.Symbol, asset1},
                    {asset2.RelatedStock.Symbol, asset2},
                    {asset3.RelatedStock.Symbol, asset3}
                }
            };
        }

        public Portfolio()
        {
            Assets = new Dictionary<string, Asset>();
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
            return new Dictionary<string, Asset>(Assets);
        }

        public override bool Equals(object obj)
        {
            var portfolio = obj as Portfolio;


            return portfolio != null &&
                   PortfolioID == portfolio.PortfolioID &&
                   Username == portfolio.Username &&
                   Password == portfolio.Password &&
                   WriteAuthority == portfolio.WriteAuthority &&
                   HasEqualAssets(this, portfolio);
            //EqualityComparer<Dictionary<string, Asset>>.Default.Equals(Assets, portfolio.Assets);
        }

        public static bool HasEqualAssets(Portfolio portfolio, Portfolio comparingPortfolio)
        {
            return portfolio.Assets.OrderBy(kvp => kvp.Key).SequenceEqual(comparingPortfolio.Assets.OrderBy(kvp => kvp.Key));
        }

        public override int GetHashCode()
        {
            var hashCode = 1633326048;
            hashCode = hashCode * -1521134295 + PortfolioID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Username);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password);
            hashCode = hashCode * -1521134295 + WriteAuthority.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, Asset>>.Default.GetHashCode(Assets);
            return hashCode;
        }
    }
}