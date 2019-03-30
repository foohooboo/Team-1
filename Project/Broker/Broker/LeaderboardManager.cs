using System.Linq;
using Shared.Leaderboard;
using Shared.MarketStructures;
using Shared.Portfolio;

namespace Broker
{
    public static class LeaderboardManager
    {
        public static MarketDay Market
        {
            get; set;
        }

        public static LeaderboardRecord GetLeaderboardRecord(Portfolio portfolio)
        {
            float totalAssetValue = 0;

            foreach (var asset in portfolio.Assets)
            {
                var stockValue = Market.TradedCompanies.Single(stock => stock.Symbol == asset.Key).Close;

                float assetValue = stockValue * asset.Value.Quantity;
                totalAssetValue += assetValue;
            }

            return new LeaderboardRecord(portfolio.Username, totalAssetValue);
        }
    }
}
